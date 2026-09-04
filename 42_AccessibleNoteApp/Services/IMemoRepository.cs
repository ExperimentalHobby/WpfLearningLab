using AccessibleNoteApp.Models;

namespace AccessibleNoteApp.Services;

/// <summary>
/// メモの永続化を抽象化する。
/// </summary>
public interface IMemoRepository
{
	/// <summary>
	/// 保存済みの全メモを、更新日時の新しい順に読み込む。
	/// </summary>
	IReadOnlyList<Memo> LoadAll();

	/// <summary>
	/// メモを保存する(同じIdが既にあれば上書き)。
	/// </summary>
	/// <param name="memo">保存するメモ。</param>
	void Save(Memo memo);

	/// <summary>
	/// 指定したIDのメモを削除する。存在しない場合は何もしない。
	/// </summary>
	/// <param name="id">削除するメモのID。</param>
	void Delete(string id);
}
