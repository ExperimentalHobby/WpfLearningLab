using System.Windows.Media.Imaging;
using ScreenCaptureTool.Services;

namespace ScreenCaptureTool.Tests.Fakes;

/// <summary>
/// テスト用の<see cref="IClipboardImageService"/>フェイク実装。
/// </summary>
public class FakeClipboardImageService : IClipboardImageService
{
	/// <summary>直近に<see cref="SetImage"/>で設定された画像(テスト用)。</summary>
	public BitmapSource? LastSetImage { get; private set; }

	/// <inheritdoc/>
	public void SetImage(BitmapSource image) => LastSetImage = image;
}
