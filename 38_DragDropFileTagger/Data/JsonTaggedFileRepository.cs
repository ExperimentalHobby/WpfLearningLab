using System.IO;
using System.Text.Json;
using DragDropFileTagger.Models;

namespace DragDropFileTagger.Data;

/// <summary>
/// JSONファイルへタグ付けファイル一覧を永続化する<see cref="ITaggedFileRepository"/>の実装。
/// </summary>
public class JsonTaggedFileRepository : ITaggedFileRepository
{
	private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

	private readonly string _filePath;

	/// <summary>
	/// <see cref="JsonTaggedFileRepository"/>を初期化する。
	/// </summary>
	/// <param name="filePath">保存先のJSONファイルパス。</param>
	public JsonTaggedFileRepository(string filePath)
	{
		_filePath = filePath;
	}

	/// <inheritdoc/>
	public List<TaggedFile> Load()
	{
		if (!File.Exists(_filePath))
		{
			return [];
		}

		try
		{
			var json = File.ReadAllText(_filePath);
			return JsonSerializer.Deserialize<List<TaggedFile>>(json) ?? [];
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
		{
			// コンストラクタでLoad()を呼ぶ都合上、ここで例外を投げると起動時クラッシュになる。
			// 保存ファイルが壊れている・読み込めない場合でも、空の状態から起動できるようにする。
			return [];
		}
	}

	/// <inheritdoc/>
	public void Save(IReadOnlyList<TaggedFile> files)
	{
		try
		{
			var directory = Path.GetDirectoryName(_filePath);
			if (!string.IsNullOrEmpty(directory))
			{
				Directory.CreateDirectory(directory);
			}
			var json = JsonSerializer.Serialize(files, SerializerOptions);
			File.WriteAllText(_filePath, json);
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
		{
			// 保存に失敗しても(ディスク容量不足・権限不足等)、タグ付け操作自体は続行できるようにする。
			System.Diagnostics.Debug.WriteLine($"タグ付けファイル一覧の保存に失敗しました: {ex.Message}");
		}
	}
}
