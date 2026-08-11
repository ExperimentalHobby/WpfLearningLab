using System.Windows.Media;

namespace ImageViewer.ViewModels;

/// <summary>
/// 一覧表示する画像1件分のViewModel。サムネイルは初期状態ではnullで、
/// 読み込み完了後に<see cref="Thumbnail"/>が非同期に設定される。
/// </summary>
public class ImageFileViewModel : ObservableObject
{
	private ImageSource? _thumbnail;

	/// <summary>
	/// 画像ファイルのViewModelを初期化する。
	/// </summary>
	/// <param name="filePath">画像ファイルのフルパス。</param>
	public ImageFileViewModel(string filePath)
	{
		FilePath = filePath;
		FileName = System.IO.Path.GetFileName(filePath);
	}

	/// <summary>画像ファイルのフルパス。</summary>
	public string FilePath { get; }

	/// <summary>ファイル名(拡張子含む)。</summary>
	public string FileName { get; }

	/// <summary>サムネイル画像。読み込み完了前は<see langword="null"/>。</summary>
	public ImageSource? Thumbnail
	{
		get => _thumbnail;
		set => SetProperty(ref _thumbnail, value);
	}
}
