using System.Diagnostics;
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
			try
			{
				var json = File.ReadAllText(filePath);
				var memo = JsonSerializer.Deserialize<Memo>(json);
				if (memo is not null)
				{
					memos.Add(memo);
				}
			}
			catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
			{
				// 1件の破損ファイル(不正なJSON等)が原因で他の正常なメモまで開けなくなることを防ぐため、
				// このファイルはスキップして読み込みを継続する。
				Debug.WriteLine($"メモファイルの読み込みに失敗しました: {filePath}, {ex.Message}");
			}
		}

		return memos.OrderByDescending(m => m.UpdatedAt).ToList();
	}

	/// <inheritdoc/>
	public void Save(Memo memo)
	{
		try
		{
			File.WriteAllText(GetFilePath(memo.Id), JsonSerializer.Serialize(memo, SerializerOptions));
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
		{
			Debug.WriteLine($"メモファイルの保存に失敗しました: {memo.Id}, {ex.Message}");
		}
	}

	/// <inheritdoc/>
	public void Delete(string id)
	{
		try
		{
			var filePath = GetFilePath(id);
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
		{
			Debug.WriteLine($"メモファイルの削除に失敗しました: {id}, {ex.Message}");
		}
	}

	private string GetFilePath(string id) => Path.Combine(_directoryPath, $"{id}.json");
}
