using FileOrganizer.Services;

namespace FileOrganizer.Tests;

/// <summary>
/// <see cref="FileSystemDirectoryWatcher"/> の単体テスト。
/// 実の一時フォルダに対して実際にファイルを作成し、<see cref="System.IO.FileSystemWatcher"/>の
/// イベントが発火することを検証する。
/// </summary>
public class FileSystemDirectoryWatcherTests : IDisposable
{
	private readonly string _watchFolder;

	public FileSystemDirectoryWatcherTests()
	{
		_watchFolder = Path.Combine(Path.GetTempPath(), $"FileOrganizerWatcherTests_{Guid.NewGuid():N}");
		Directory.CreateDirectory(_watchFolder);
	}

	public void Dispose()
	{
		if (Directory.Exists(_watchFolder))
		{
			Directory.Delete(_watchFolder, recursive: true);
		}
	}

	/// <summary>
	/// パス条件: 監視中のフォルダ直下にファイルを作成すると、FileCreatedイベントが発火すること
	/// </summary>
	[Fact]
	public async Task Start_フォルダ直下にファイルを作成するとFileCreatedが発火する()
	{
		using var watcher = new FileSystemDirectoryWatcher();
		var tcs = new TaskCompletionSource<string>();
		watcher.FileCreated += path => tcs.TrySetResult(path);
		watcher.Start(_watchFolder);

		var filePath = Path.Combine(_watchFolder, "new.txt");
		File.WriteAllText(filePath, "dummy");

		var completed = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(5)));

		Assert.Same(tcs.Task, completed);
		Assert.Equal(filePath, await tcs.Task);
	}
}
