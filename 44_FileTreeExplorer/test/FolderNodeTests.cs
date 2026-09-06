using FileTreeExplorer.Models;

namespace FileTreeExplorer.Tests;

/// <summary>
/// <see cref="FolderNode"/> のテスト。
/// </summary>
public class FolderNodeTests
{
	/// <summary>
	/// パス条件: LoadChildrenが成功すると、IsLoadedがtrueになり子ノードが設定されること
	/// </summary>
	[Fact]
	public void LoadChildren_成功するとIsLoadedがtrueになり子ノードが設定される()
	{
		var fileSystem = new FakeFileSystem();
		fileSystem.SetDirectories(@"C:\parent", [@"C:\parent\a", @"C:\parent\b"]);
		var engine = new FileSystemBrowserEngine(fileSystem);
		var node = new FolderNode("parent", @"C:\parent");

		node.LoadChildren(engine, out var errorMessage);

		Assert.True(node.IsLoaded);
		Assert.Null(errorMessage);
		Assert.Equal(2, node.Children.Count);
	}

	/// <summary>
	/// パス条件: LoadChildrenがアクセス拒否等で失敗した場合、IsLoadedはfalseのままであること
	/// (失敗を「読み込み済み」扱いしてしまい、再展開しても再試行されないクラッシュではないが実害のある不具合の回帰テスト)。
	/// </summary>
	[Fact]
	public void LoadChildren_失敗するとIsLoadedはfalseのままになる()
	{
		var fileSystem = new FakeFileSystem();
		fileSystem.SetDirectoryException(@"C:\denied", new UnauthorizedAccessException());
		var engine = new FileSystemBrowserEngine(fileSystem);
		var node = new FolderNode("denied", @"C:\denied");

		node.LoadChildren(engine, out var errorMessage);

		Assert.False(node.IsLoaded);
		Assert.NotNull(errorMessage);
	}

	/// <summary>
	/// パス条件: LoadChildren失敗後に原因が解消してから再度LoadChildrenを呼び出すと、
	/// 正しく子ノードが読み込まれること(再試行できることの回帰テスト)。
	/// </summary>
	[Fact]
	public void LoadChildren_失敗後に再度呼び出すと再試行される()
	{
		var fileSystem = new FakeFileSystem();
		fileSystem.SetDirectoryException(@"C:\denied", new UnauthorizedAccessException());
		var engine = new FileSystemBrowserEngine(fileSystem);
		var node = new FolderNode("denied", @"C:\denied");
		node.LoadChildren(engine, out _);

		fileSystem.ClearDirectoryException(@"C:\denied");
		fileSystem.SetDirectories(@"C:\denied", [@"C:\denied\a"]);
		node.LoadChildren(engine, out var errorMessage);

		Assert.True(node.IsLoaded);
		Assert.Null(errorMessage);
		Assert.Single(node.Children);
	}

	/// <summary>
	/// パス条件: LoadChildren失敗時、子ノードにプレースホルダーが1件復元されること
	/// (プレースホルダーが無いとTreeViewItemの展開矢印が消え、UI上再展開する手段が無くなるための回帰テスト)。
	/// </summary>
	[Fact]
	public void LoadChildren_失敗時はプレースホルダーが復元される()
	{
		var fileSystem = new FakeFileSystem();
		fileSystem.SetDirectoryException(@"C:\denied", new UnauthorizedAccessException());
		var engine = new FileSystemBrowserEngine(fileSystem);
		var node = new FolderNode("denied", @"C:\denied");

		node.LoadChildren(engine, out _);

		var placeholder = Assert.Single(node.Children);
		Assert.True(placeholder.IsPlaceholder);
	}
}
