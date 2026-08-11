namespace ImageViewer.Services;

/// <summary>
/// フォルダ内の画像ファイルを列挙する処理の抽象。
/// </summary>
public interface IImageFileScanner
{
	/// <summary>
	/// 指定したフォルダ内の画像ファイル(jpg/jpeg/png/bmp/gif)のパスを取得する。
	/// </summary>
	/// <param name="folderPath">走査するフォルダのパス。</param>
	IReadOnlyList<string> GetImageFilePaths(string folderPath);
}
