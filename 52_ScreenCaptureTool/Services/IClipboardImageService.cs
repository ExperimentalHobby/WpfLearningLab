using System.Windows.Media.Imaging;

namespace ScreenCaptureTool.Services;

/// <summary>
/// 画像のクリップボードコピーを担う抽象。
/// </summary>
public interface IClipboardImageService
{
	/// <summary>
	/// 指定した画像をクリップボードにコピーする。
	/// </summary>
	/// <param name="image">コピーする画像。</param>
	void SetImage(BitmapSource image);
}
