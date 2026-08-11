using System.Windows.Media;

namespace ImageViewer.Services;

/// <summary>
/// 画像ファイルからサムネイル(縮小画像)を生成する処理の抽象。
/// </summary>
public interface IThumbnailLoader
{
	/// <summary>
	/// 指定したファイルのサムネイルを非同期で読み込む。
	/// </summary>
	/// <param name="filePath">画像ファイルのパス。</param>
	Task<ImageSource?> LoadAsync(string filePath);
}
