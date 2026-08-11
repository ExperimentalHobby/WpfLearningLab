using MarkdownMemo.Data;

namespace MarkdownMemo.Tests;

/// <summary>
/// <see cref="FileMemoRepository"/> の単体テスト。
/// テストごとに実の一時フォルダへアクセスし、CRUDを検証する。
/// </summary>
public class FileMemoRepositoryTests : IDisposable
{
	private readonly string _folderPath;

	public FileMemoRepositoryTests()
	{
		_folderPath = Path.Combine(Path.GetTempPath(), $"MarkdownMemoTests_{Guid.NewGuid():N}");
	}

	public void Dispose()
	{
		if (Directory.Exists(_folderPath))
		{
			Directory.Delete(_folderPath, recursive: true);
		}
	}

	/// <summary>
	/// パス条件: Saveで保存したメモがGetAllの一覧に反映されること
	/// </summary>
	[Fact]
	public void Save_保存したメモがGetAllの一覧に反映される()
	{
		var repository = new FileMemoRepository(_folderPath);

		repository.Save("買い物メモ", "- 牛乳\n- 卵");
		var all = repository.GetAll();

		Assert.Single(all);
		Assert.Equal("買い物メモ", all[0].Title);
	}

	/// <summary>
	/// パス条件: Saveで保存した内容をLoadで復元できること
	/// </summary>
	[Fact]
	public void Save_保存した内容がLoadで復元できる()
	{
		var repository = new FileMemoRepository(_folderPath);

		repository.Save("買い物メモ", "- 牛乳\n- 卵");
		var content = repository.Load("買い物メモ");

		Assert.Equal("- 牛乳\n- 卵", content);
	}

	/// <summary>
	/// パス条件: Deleteで削除したメモがGetAllの一覧から消えること
	/// </summary>
	[Fact]
	public void Delete_削除したメモがGetAllから消える()
	{
		var repository = new FileMemoRepository(_folderPath);
		repository.Save("買い物メモ", "- 牛乳");

		repository.Delete("買い物メモ");
		var all = repository.GetAll();

		Assert.Empty(all);
	}

	/// <summary>
	/// パス条件: 同じタイトルでSaveを再度呼ぶと、新規作成されず内容が上書き更新されること
	/// </summary>
	[Fact]
	public void Save_同じタイトルで再度呼ぶと上書き更新される()
	{
		var repository = new FileMemoRepository(_folderPath);
		repository.Save("買い物メモ", "- 牛乳");

		repository.Save("買い物メモ", "- 卵");
		var all = repository.GetAll();
		var content = repository.Load("買い物メモ");

		Assert.Single(all);
		Assert.Equal("- 卵", content);
	}
}
