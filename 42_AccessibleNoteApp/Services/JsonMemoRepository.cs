using System.IO;
using System.Text.Json;
using AccessibleNoteApp.Models;

namespace AccessibleNoteApp.Services;

/// <summary>
/// 1メモ1JSONファイルとして実際のファイルシステムに保存する実装。
/// </summary>
public sealed class JsonMemoRepository : IMemoRepository
{
	private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

	private readonly string _directoryPath;

	/// <summary>
	/// 保存先フォルダを指定して初期化する。フォルダが存在しない場合は作成する。
	/// </summary>
	/// <param name="directoryPath">保存先フォルダのパス。</param>
	public JsonMemoRepository(string directoryPath)
	{
		_directoryPath = directoryPath;
		Directory.CreateDirectory(_directoryPath);
	}

	/// <inheritdoc/>
	public IReadOnlyList<Memo> LoadAll()
	{
		var memos = new List<Memo>();
		foreach (var filePath in Directory.EnumerateFiles(_directoryPath, "*.json"))
		{
			var json = File.ReadAllText(filePath);
			var memo = JsonSerializer.Deserialize<Memo>(json);
			if (memo is not null)
			{
				memos.Add(memo);
			}
		}

		return memos.OrderByDescending(m => m.UpdatedAt).ToList();
	}

	/// <inheritdoc/>
	public void Save(Memo memo)
	{
		File.WriteAllText(GetFilePath(memo.Id), JsonSerializer.Serialize(memo, SerializerOptions));
	}

	/// <inheritdoc/>
	public void Delete(string id)
	{
		var filePath = GetFilePath(id);
		if (File.Exists(filePath))
		{
			File.Delete(filePath);
		}
	}

	private string GetFilePath(string id) => Path.Combine(_directoryPath, $"{id}.json");
}
