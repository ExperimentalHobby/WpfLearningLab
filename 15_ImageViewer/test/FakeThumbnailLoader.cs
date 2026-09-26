using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageViewer.Services;

namespace ImageViewer.Tests;

/// <summary>
/// <see cref="ImageViewer.ViewModels.MainViewModel"/> のテスト用に、実ファイルをデコードしない<see cref="IThumbnailLoader"/>実装。
/// 呼び出されたファイルパスを記録し、ダミーの<see cref="ImageSource"/>を返す。
/// </summary>
public class FakeThumbnailLoader : IThumbnailLoader
{
	/// <summary><see cref="LoadAsync"/>で要求されたファイルパスの履歴(テスト用)。</summary>
	public List<string> RequestedFilePaths { get; } = [];

	/// <summary>設定すると<see cref="LoadAsync"/>がこのTaskの完了まで待機する(テスト用)。</summary>
	public TaskCompletionSource? Gate { get; set; }

	/// <inheritdoc/>
	public async Task<ImageSource?> LoadAsync(string filePath)
	{
		RequestedFilePaths.Add(filePath);
		if (Gate is not null)
		{
			await Gate.Task;
		}

		return new BitmapImage();
	}
}
