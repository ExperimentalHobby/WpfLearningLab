using System.Windows.Media.Imaging;
using ScreenCaptureTool.Services;

namespace ScreenCaptureTool.Tests.Fakes;

/// <summary>
/// テスト用の<see cref="IFileSaveService"/>フェイク実装。
/// </summary>
public class FakeFileSaveService : IFileSaveService
{
	/// <summary>直近に<see cref="Save"/>で保存された画像(テスト用)。</summary>
	public BitmapSource? LastSavedImage { get; private set; }

	/// <summary>直近に<see cref="Save"/>で保存されたパス(テスト用)。</summary>
	public string? LastSavedPath { get; private set; }

	/// <inheritdoc/>
	public void Save(BitmapSource image, string path)
	{
		LastSavedImage = image;
		LastSavedPath = path;
	}
}
