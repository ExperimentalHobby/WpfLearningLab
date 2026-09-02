using MusicPlayer.Services;

namespace MusicPlayer.Tests;

/// <summary>
/// <see cref="AudioFileScanner"/> の単体テスト。
/// テストごとに実の一時フォルダへファイルを作成し、ファイルシステム操作を実際に検証する。
/// </summary>
public class AudioFileScannerTests : IDisposable
{
	private readonly string _folderPath;

	public AudioFileScannerTests()
	{
		_folderPath = Path.Combine(Path.GetTempPath(), $"MusicPlayerTests_{Guid.NewGuid():N}");
		Directory.CreateDirectory(_folderPath);
	}

	public void Dispose()
	{
		if (Directory.Exists(_folderPath))
		{
			Directory.Delete(_folderPath, recursive: true);
		}
	}

	/// <summary>
	/// パス条件: 対象拡張子(mp3/wav/wma)のファイルのみが取得できること
	/// </summary>
	[Fact]
	public async Task GetAudioFilePathsAsync_対象拡張子のファイルのみ取得できる()
	{
		File.WriteAllText(Path.Combine(_folderPath, "曲A.mp3"), string.Empty);
		File.WriteAllText(Path.Combine(_folderPath, "曲B.wav"), string.Empty);
		File.WriteAllText(Path.Combine(_folderPath, "曲C.wma"), string.Empty);
		File.WriteAllText(Path.Combine(_folderPath, "メモ.txt"), string.Empty);
		var scanner = new AudioFileScanner();

		var paths = await scanner.GetAudioFilePathsAsync(_folderPath);

		Assert.Equal(3, paths.Count);
		Assert.DoesNotContain(paths, p => p.EndsWith(".txt"));
	}

	/// <summary>
	/// パス条件: フォルダが空の場合は空の一覧を返すこと
	/// </summary>
	[Fact]
	public async Task GetAudioFilePathsAsync_フォルダが空の場合空の一覧を返す()
	{
		var scanner = new AudioFileScanner();

		var paths = await scanner.GetAudioFilePathsAsync(_folderPath);

		Assert.Empty(paths);
	}

	/// <summary>
	/// パス条件: 存在しないフォルダを指定した場合は空の一覧を返すこと
	/// </summary>
	[Fact]
	public async Task GetAudioFilePathsAsync_存在しないフォルダの場合空の一覧を返す()
	{
		var scanner = new AudioFileScanner();

		var paths = await scanner.GetAudioFilePathsAsync(Path.Combine(_folderPath, "存在しない"));

		Assert.Empty(paths);
	}
}
