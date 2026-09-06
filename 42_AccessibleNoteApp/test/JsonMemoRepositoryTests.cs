using AccessibleNoteApp.Models;
using AccessibleNoteApp.Services;

namespace AccessibleNoteApp.Tests;

/// <summary>
/// <see cref="JsonMemoRepository"/> の単体テスト。実際の一時フォルダに対して検証する(モック不使用)。
/// </summary>
public class JsonMemoRepositoryTests : IDisposable
{
	private readonly string _tempDirectory;

	public JsonMemoRepositoryTests()
	{
		_tempDirectory = Path.Combine(Path.GetTempPath(), $"AccessibleNoteAppTests_{Guid.NewGuid():N}");
	}

	public void Dispose()
	{
		if (Directory.Exists(_tempDirectory))
		{
			Directory.Delete(_tempDirectory, recursive: true);
		}
	}

	/// <summary>
	/// パス条件: 保存したメモをLoadAllで読み込めること
	/// </summary>
	[Fact]
	public void Save_保存したメモをLoadAllで読み込める()
	{
		var repository = new JsonMemoRepository(_tempDirectory);
		var memo = new Memo("id-1", "タイトル", "本文", new DateTime(2026, 1, 1));

		repository.Save(memo);
		var loaded = repository.LoadAll();

		var result = Assert.Single(loaded);
		Assert.Equal(memo, result);
	}

	/// <summary>
	/// パス条件: 複数保存したメモが、更新日時の新しい順にLoadAllで返ること
	/// </summary>
	[Fact]
	public void LoadAll_複数保存すると更新日時の新しい順に返る()
	{
		var repository = new JsonMemoRepository(_tempDirectory);
		var older = new Memo("id-old", "古いメモ", "本文", new DateTime(2026, 1, 1));
		var newer = new Memo("id-new", "新しいメモ", "本文", new DateTime(2026, 1, 2));
		repository.Save(older);
		repository.Save(newer);

		var loaded = repository.LoadAll();

		Assert.Equal(["id-new", "id-old"], loaded.Select(m => m.Id));
	}

	/// <summary>
	/// パス条件: 同じIdで再度保存すると上書きされること
	/// </summary>
	[Fact]
	public void Save_同じIdで再度保存すると上書きされる()
	{
		var repository = new JsonMemoRepository(_tempDirectory);
		repository.Save(new Memo("id-1", "旧タイトル", "旧本文", new DateTime(2026, 1, 1)));

		repository.Save(new Memo("id-1", "新タイトル", "新本文", new DateTime(2026, 1, 2)));
		var loaded = repository.LoadAll();

		var result = Assert.Single(loaded);
		Assert.Equal("新タイトル", result.Title);
	}

	/// <summary>
	/// パス条件: Deleteで指定したメモが削除されること
	/// </summary>
	[Fact]
	public void Delete_指定したメモが削除される()
	{
		var repository = new JsonMemoRepository(_tempDirectory);
		repository.Save(new Memo("id-1", "タイトル", "本文", new DateTime(2026, 1, 1)));

		repository.Delete("id-1");
		var loaded = repository.LoadAll();

		Assert.Empty(loaded);
	}

	/// <summary>
	/// パス条件: 存在しないIDをDeleteしても例外にならないこと
	/// </summary>
	[Fact]
	public void Delete_存在しないIDを指定しても例外にならない()
	{
		var repository = new JsonMemoRepository(_tempDirectory);

		var exception = Record.Exception(() => repository.Delete("not-exist"));

		Assert.Null(exception);
	}

	/// <summary>
	/// パス条件: 壊れたJSONファイルが1件混ざっていても、例外にならず他の正常なメモは読み込めること
	/// (1件の破損ファイルが原因で起動時に全メモが開けなくなるクラッシュの回帰テスト)。
	/// </summary>
	[Fact]
	public void LoadAll_壊れたJSONファイルが混ざっていても正常なメモは読み込める()
	{
		var repository = new JsonMemoRepository(_tempDirectory);
		repository.Save(new Memo("id-ok", "正常なメモ", "本文", new DateTime(2026, 1, 1)));
		File.WriteAllText(Path.Combine(_tempDirectory, "id-broken.json"), "{ this is not valid json");

		var exception = Record.Exception(() => repository.LoadAll());

		Assert.Null(exception);
		var loaded = repository.LoadAll();
		var result = Assert.Single(loaded);
		Assert.Equal("id-ok", result.Id);
	}
}
