namespace FileTreeExplorer.Tests;

/// <summary>
/// <see cref="FileSystemBrowserEngine"/> のテスト。
/// </summary>
public class FileSystemBrowserEngineTests
{
    /// <summary>
    /// パス条件: サブフォルダが存在するパスに対してTryGetSubFoldersを呼ぶと成功しフォルダ一覧を返すこと
    /// </summary>
    [Fact]
    public void TryGetSubFolders_正常時はサブフォルダ一覧を返す()
    {
        var fakeFileSystem = new FakeFileSystem();
        fakeFileSystem.SetDirectories(@"C:\Work", new[] { @"C:\Work\A", @"C:\Work\B" });
        var engine = new FileSystemBrowserEngine(fakeFileSystem);

        var success = engine.TryGetSubFolders(@"C:\Work", out var folders, out var errorMessage);

        Assert.True(success);
        Assert.Equal(new[] { @"C:\Work\A", @"C:\Work\B" }, folders);
        Assert.Null(errorMessage);
    }

    /// <summary>
    /// パス条件: アクセス拒否例外が発生するパスに対してTryGetSubFoldersを呼ぶと
    /// 失敗し、空一覧とエラーメッセージを返すこと(例外でアプリを落とさない)
    /// </summary>
    [Fact]
    public void TryGetSubFolders_アクセス拒否例外時は失敗しエラーメッセージを返す()
    {
        var fakeFileSystem = new FakeFileSystem();
        fakeFileSystem.SetDirectoryException(@"C:\Protected", new UnauthorizedAccessException());
        var engine = new FileSystemBrowserEngine(fakeFileSystem);

        var success = engine.TryGetSubFolders(@"C:\Protected", out var folders, out var errorMessage);

        Assert.False(success);
        Assert.Empty(folders);
        Assert.Equal("アクセスが拒否されました。", errorMessage);
    }

    /// <summary>
    /// パス条件: ファイルが存在するパスに対してTryGetFilesを呼ぶと成功しファイル一覧を返すこと
    /// </summary>
    [Fact]
    public void TryGetFiles_正常時はファイル一覧を返す()
    {
        var fakeFileSystem = new FakeFileSystem();
        var expected = new[] { new FileEntry("a.txt", 100, new DateTime(2026, 1, 1)) };
        fakeFileSystem.SetFiles(@"C:\Work", expected);
        var engine = new FileSystemBrowserEngine(fakeFileSystem);

        var success = engine.TryGetFiles(@"C:\Work", out var files, out var errorMessage);

        Assert.True(success);
        Assert.Equal(expected, files);
        Assert.Null(errorMessage);
    }

    /// <summary>
    /// パス条件: アクセス拒否例外が発生するパスに対してTryGetFilesを呼ぶと
    /// 失敗し、空一覧とエラーメッセージを返すこと
    /// </summary>
    [Fact]
    public void TryGetFiles_アクセス拒否例外時は失敗しエラーメッセージを返す()
    {
        var fakeFileSystem = new FakeFileSystem();
        fakeFileSystem.SetFileException(@"C:\Protected", new UnauthorizedAccessException());
        var engine = new FileSystemBrowserEngine(fakeFileSystem);

        var success = engine.TryGetFiles(@"C:\Protected", out var files, out var errorMessage);

        Assert.False(success);
        Assert.Empty(files);
        Assert.Equal("アクセスが拒否されました。", errorMessage);
    }
}
