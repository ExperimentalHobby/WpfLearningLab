using System.Windows.Media.Imaging;
using ScreenCaptureTool.Services;

namespace ScreenCaptureTool.Tests.Fakes;

/// <summary>
/// テスト用の<see cref="IFileSaveService"/>フェイク実装。
/// </summary>
public class FakeFileSaveService : IFileSaveService
{
	public BitmapSource? LastSavedImage { get; private set; }

	public string? LastSavedPath { get; private set; }

	public void Save(BitmapSource image, string path)
	{
		LastSavedImage = image;
		LastSavedPath = path;
	}
}
