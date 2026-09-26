using ImageViewer.Services;

namespace ImageViewer.Tests;

/// <summary>
/// <see cref="ImageViewer.ViewModels.MainViewModel"/> のテスト用に、実ファイルシステムを使わない<see cref="IImageFileScanner"/>実装。
/// </summary>
public class FakeImageFileScanner : IImageFileScanner
{
	/// <summary><see cref="GetImageFilePaths"/>が返す値(テスト用)。</summary>
	public IReadOnlyList<string> FilePathsToReturn { get; set; } = [];

	/// <summary>設定すると<see cref="GetImageFilePaths"/>呼び出し時にこの例外をスローする(テスト用)。</summary>
	public Exception? ExceptionToThrow { get; set; }

	/// <inheritdoc/>
	public IReadOnlyList<string> GetImageFilePaths(string folderPath)
	{
		if (ExceptionToThrow is not null)
		{
			throw ExceptionToThrow;
		}

		return FilePathsToReturn;
	}
}
