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
		var json = File.ReadAllText(_filePath);
		return JsonSerializer.Deserialize<List<TaggedFile>>(json) ?? [];
	}

	/// <inheritdoc/>
	public void Save(IReadOnlyList<TaggedFile> files)
	{
		var directory = Path.GetDirectoryName(_filePath);
		if (!string.IsNullOrEmpty(directory))
		{
			Directory.CreateDirectory(directory);
		}
		var json = JsonSerializer.Serialize(files, SerializerOptions);
		File.WriteAllText(_filePath, json);
	}
}
