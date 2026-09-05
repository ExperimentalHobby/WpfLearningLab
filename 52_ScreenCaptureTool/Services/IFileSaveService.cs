using System.Windows.Media.Imaging;

namespace ScreenCaptureTool.Services;

/// <summary>
/// 画像のファイル保存を担う抽象。
/// </summary>
public interface IFileSaveService
{
	/// <summary>
	/// 指定した画像を指定パスにPNG形式で保存する。
	/// </summary>
	/// <param name="image">保存する画像。</param>
	/// <param name="path">保存先のファイルパス。</param>
	void Save(BitmapSource image, string path);
}
