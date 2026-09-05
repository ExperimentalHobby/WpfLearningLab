using System.IO;
using System.Windows.Media.Imaging;

namespace ScreenCaptureTool.Services;

/// <summary>
/// PNG形式でのファイル保存を行う<see cref="IFileSaveService"/>の実装。
/// </summary>
public class PngFileSaveService : IFileSaveService
{
	/// <inheritdoc/>
	public void Save(BitmapSource image, string path)
	{
		var encoder = new PngBitmapEncoder();
		encoder.Frames.Add(BitmapFrame.Create(image));

		using var stream = File.Create(path);
		encoder.Save(stream);
	}
}
