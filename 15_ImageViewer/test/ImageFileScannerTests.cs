using ImageViewer.Services;

namespace ImageViewer.Tests;

/// <summary>
/// <see cref="ImageFileScanner"/> の単体テスト。
/// テストごとに一時フォルダへ実際にファイルを作成して検証する(ファイルシステム操作は高速・決定的なため実I/Oでテストする)。
/// </summary>
public class ImageFileScannerTests : IDisposable
{
	private readonly string _tempDir;

	public ImageFileScannerTests()
	{
		_tempDir = Path.Combine(Path.GetTempPath(), $"ImageViewerTests_{Guid.NewGuid():N}");
		Directory.CreateDirectory(_tempDir);
	}

	public void Dispose()
	{
		if (Directory.Exists(_tempDir))
		{
			Directory.Delete(_tempDir, recursive: true);
		}
	}

	/// <summary>
	/// パス条件: フォルダ内の画像ファイル(jpg/png/bmp/gif)のみを取得できること
	/// </summary>
	[Fact]
	public void GetImageFilePaths_画像ファイルのみを取得できる()
	{
		File.WriteAllText(Path.Combine(_tempDir, "photo1.jpg"), "dummy");
		File.WriteAllText(Path.Combine(_tempDir, "photo2.png"), "dummy");
		File.WriteAllText(Path.Combine(_tempDir, "memo.txt"), "dummy");
		var scanner = new ImageFileScanner();

		var result = scanner.GetImageFilePaths(_tempDir);

		Assert.Equal(2, result.Count);
		Assert.Contains(result, p => p.EndsWith("photo1.jpg"));
		Assert.Contains(result, p => p.EndsWith("photo2.png"));
	}

	/// <summary>
	/// パス条件: 画像でないファイル(.txt等)は結果に含まれないこと
	/// </summary>
	[Fact]
	public void GetImageFilePaths_画像でないファイルは除外される()
	{
		File.WriteAllText(Path.Combine(_tempDir, "memo.txt"), "dummy");
		File.WriteAllText(Path.Combine(_tempDir, "data.csv"), "dummy");
		var scanner = new ImageFileScanner();

		var result = scanner.GetImageFilePaths(_tempDir);

		Assert.Empty(result);
	}

	/// <summary>
	/// パス条件: フォルダが空の場合、空リストを返すこと
	/// </summary>
	[Fact]
	public void GetImageFilePaths_フォルダが空の場合空リストを返す()
	{
		var scanner = new ImageFileScanner();

		var result = scanner.GetImageFilePaths(_tempDir);

		Assert.Empty(result);
	}
}
