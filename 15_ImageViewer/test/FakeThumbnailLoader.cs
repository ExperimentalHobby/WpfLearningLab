using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageViewer.Services;

namespace ImageViewer.Tests;

/// <summary>
/// <see cref="MainViewModel"/> のテスト用に、実ファイルをデコードしない<see cref="IThumbnailLoader"/>実装。
/// 呼び出された<see cref="FilePath"/>を記録し、ダミーの<see cref="ImageSource"/>を返す。
/// </summary>
public class FakeThumbnailLoader : IThumbnailLoader
{
	public List<string> RequestedFilePaths { get; } = [];
	public TaskCompletionSource? Gate { get; set; }

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
