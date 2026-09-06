using DragDropFileTagger.Data;
using DragDropFileTagger.Models;

namespace DragDropFileTagger.Tests;

/// <summary>
/// <see cref="JsonTaggedFileRepository"/>のテスト。実際の一時ファイルへ読み書きして検証する。
/// </summary>
public class JsonTaggedFileRepositoryTests : IDisposable
{
	private readonly string _filePath;

	public JsonTaggedFileRepositoryTests()
	{
		_filePath = Path.Combine(Path.GetTempPath(), $"DragDropFileTaggerTests_{Guid.NewGuid():N}.json");
	}

	public void Dispose()
	{
		if (File.Exists(_filePath))
		{
			File.Delete(_filePath);
		}
	}

	/// <summary>
	/// パス条件: 保存したファイル一覧が、読み込み時に同じ内容(パス・タグ・並び順)で復元されること。
	/// </summary>
	[Fact]
	public void SaveAndLoad_保存した内容が復元される()
	{
		var repository = new JsonTaggedFileRepository(_filePath);
		var files = new List<TaggedFile>
		{
			new() { FilePath = @"C:\a.txt", SizeBytes = 100, Tags = ["重要"], SortOrder = 0 },
			new() { FilePath = @"C:\b.txt", SizeBytes = 200, Tags = ["仕事", "重要"], SortOrder = 1 },
		};

		repository.Save(files);
		var loaded = repository.Load();

		Assert.Equal(2, loaded.Count);
		Assert.Equal(@"C:\a.txt", loaded[0].FilePath);
		Assert.Equal(["仕事", "重要"], loaded[1].Tags);
	}

	/// <summary>
	/// パス条件: 保存前(ファイルが存在しない場合)は空のリストを返すこと。
	/// </summary>
	[Fact]
	public void Load_ファイルが存在しない場合は空のリストを返す()
	{
		var repository = new JsonTaggedFileRepository(_filePath);

		Assert.Empty(repository.Load());
	}

	/// <summary>
	/// パス条件: 壊れたJSONファイルを読み込んでも例外を投げず、空のリストを返すこと
	/// (コンストラクタでLoad()を呼ぶため、ここで例外を投げると起動時クラッシュになる)。
	/// </summary>
	[Fact]
	public void Load_壊れたJSONファイルは例外を投げず空のリストを返す()
	{
		File.WriteAllText(_filePath, "{ this is not valid json");
		var repository = new JsonTaggedFileRepository(_filePath);

		var result = repository.Load();

		Assert.Empty(result);
	}
}
