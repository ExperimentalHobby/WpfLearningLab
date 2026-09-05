using System.Windows.Media.Imaging;
using ScreenCaptureTool.Services;

namespace ScreenCaptureTool.Tests.Fakes;

/// <summary>
/// テスト用の<see cref="IClipboardImageService"/>フェイク実装。
/// </summary>
public class FakeClipboardImageService : IClipboardImageService
{
	public BitmapSource? LastSetImage { get; private set; }

	public void SetImage(BitmapSource image) => LastSetImage = image;
}
