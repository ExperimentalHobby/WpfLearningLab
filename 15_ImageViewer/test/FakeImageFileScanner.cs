using ImageViewer.Services;

namespace ImageViewer.Tests;

/// <summary>
/// <see cref="MainViewModel"/> のテスト用に、実ファイルシステムを使わない<see cref="IImageFileScanner"/>実装。
/// </summary>
public class FakeImageFileScanner : IImageFileScanner
{
	public IReadOnlyList<string> FilePathsToReturn { get; set; } = [];

	public IReadOnlyList<string> GetImageFilePaths(string folderPath) => FilePathsToReturn;
}
