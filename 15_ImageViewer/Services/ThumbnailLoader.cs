using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageViewer.Services;

/// <summary>
/// バックグラウンドスレッドで縮小デコードしたサムネイルを生成する<see cref="IThumbnailLoader"/>実装。
/// UIスレッドをブロックしないよう<see cref="Task.Run(Action)"/>内でデコードし、
/// 生成した<see cref="BitmapImage"/>は<see cref="Freezable.Freeze"/>してスレッド間で安全に受け渡す。
/// </summary>
public class ThumbnailLoader : IThumbnailLoader
{
	private const int ThumbnailDecodePixelWidth = 120;

	/// <inheritdoc/>
	public Task<ImageSource?> LoadAsync(string filePath) =>
		Task.Run<ImageSource?>(() =>
		{
			var bitmap = new BitmapImage();
			bitmap.BeginInit();
			bitmap.UriSource = new Uri(filePath);
			bitmap.DecodePixelWidth = ThumbnailDecodePixelWidth;
			bitmap.CacheOption = BitmapCacheOption.OnLoad;
			bitmap.EndInit();
			bitmap.Freeze();
			return bitmap;
		});
}
