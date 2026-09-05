using Microsoft.Win32;

namespace ScreenCaptureTool.Services;

/// <summary>
/// <see cref="SaveFileDialog"/>を使った<see cref="ISaveFileDialogService"/>の実装。
/// </summary>
public class WpfSaveFileDialogService : ISaveFileDialogService
{
	/// <inheritdoc/>
	public bool TryGetSavePath(out string? path)
	{
		var dialog = new SaveFileDialog
		{
			Filter = "PNG画像 (*.png)|*.png",
			DefaultExt = ".png",
			FileName = $"capture_{DateTime.Now:yyyyMMdd_HHmmss}.png",
		};

		if (dialog.ShowDialog() == true)
		{
			path = dialog.FileName;
			return true;
		}

		path = null;
		return false;
	}
}
