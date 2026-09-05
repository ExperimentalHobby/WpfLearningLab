using System.IO;
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
			try
			{
				var bitmap = new BitmapImage();
				bitmap.BeginInit();
				bitmap.UriSource = new Uri(filePath);
				bitmap.DecodePixelWidth = ThumbnailDecodePixelWidth;
				bitmap.CacheOption = BitmapCacheOption.OnLoad;
				bitmap.EndInit();
				bitmap.Freeze();
				return bitmap;
			}
			catch (Exception ex) when (ex is NotSupportedException or IOException or FileFormatException)
			{
				// 破損画像・拡張子偽装ファイル(例: テキストファイルに.jpg拡張子を付けたもの)
				// では、デコード時にNotSupportedException/FileFormatExceptionが、
				// ファイルが見つからない場合はIOException派生の例外が送出される。
				// LoadAsyncは「安全に失敗する(nullを返す)」ことを責務とし、呼び出し元が
				// 1件ずつ例外処理を書かなくても、1件の破損画像がフォルダ全体の読込を
				// 止めないようにする。
				return null;
			}
		});
}
