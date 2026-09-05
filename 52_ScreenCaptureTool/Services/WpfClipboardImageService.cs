using System.Windows;
using System.Windows.Media.Imaging;

namespace ScreenCaptureTool.Services;

/// <summary>
/// WPFの<see cref="Clipboard"/>を使った<see cref="IClipboardImageService"/>の実装。
/// </summary>
public class WpfClipboardImageService : IClipboardImageService
{
	/// <inheritdoc/>
	public void SetImage(BitmapSource image) => Clipboard.SetImage(image);
}
