using SelfHostedNoteSync.Server.Models;

namespace SelfHostedNoteSync.Server;

/// <summary>
/// メモをプロセス内メモリ上で保持するストア。
/// 複数のHTTPリクエストから同時にアクセスされるため、操作は排他制御する。
/// </summary>
public sealed class NoteStore
{
	private readonly Dictionary<int, Note> _notes = [];
	private readonly Lock _gate = new();
	private int _nextId = 1;

	/// <summary>
	/// 保持している全メモをID昇順で取得する。
	/// </summary>
	public IReadOnlyList<Note> GetAll()
	{
		lock (_gate)
		{
			return [.. _notes.Values.OrderBy(n => n.Id)];
		}
	}

	/// <summary>
	/// 指定IDのメモを取得する。
	/// </summary>
	/// <param name="id">メモのID。</param>
	/// <returns>該当メモ。存在しない場合は<see langword="null"/>。</returns>
	public Note? GetById(int id)
	{
		lock (_gate)
		{
			return _notes.TryGetValue(id, out var note) ? note : null;
		}
	}

	/// <summary>
	/// メモを新規追加し、IDを採番する。
	/// </summary>
	/// <param name="note">追加するメモ(Idは無視される)。</param>
	/// <returns>採番されたIDを持つ追加後のメモ。</returns>
	public Note Add(Note note)
	{
		lock (_gate)
		{
			note.Id = _nextId++;
			_notes[note.Id] = note;
			return note;
		}
	}

	/// <summary>
	/// 指定IDのメモを更新する。
	/// </summary>
	/// <param name="id">更新対象のID。</param>
	/// <param name="note">更新後の内容(Idは<paramref name="id"/>で上書きされる)。</param>
	/// <returns>更新できた場合は<see langword="true"/>。IDが存在しない場合は<see langword="false"/>。</returns>
	public bool Update(int id, Note note)
	{
		lock (_gate)
		{
			if (!_notes.ContainsKey(id))
			{
				return false;
			}

			note.Id = id;
			_notes[id] = note;
			return true;
		}
	}

	/// <summary>
	/// 指定IDのメモを削除する。
	/// </summary>
	/// <param name="id">削除対象のID。</param>
	/// <returns>削除できた場合は<see langword="true"/>。IDが存在しない場合は<see langword="false"/>。</returns>
	public bool Delete(int id)
	{
		lock (_gate)
		{
			return _notes.Remove(id);
		}
	}
}
