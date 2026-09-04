using Microsoft.Win32;

namespace PaintTool.Services;

/// <summary>
/// Win32の<see cref="SaveFileDialog"/>を使った<see cref="ISaveFileDialogService"/>の実装。
/// </summary>
public class Win32SaveFileDialogService : ISaveFileDialogService
{
	/// <inheritdoc/>
	public string? PromptForSavePath(string defaultExtension, string filter)
	{
		var dialog = new SaveFileDialog
		{
			DefaultExt = defaultExtension,
			Filter = filter,
		};

		return dialog.ShowDialog() == true ? dialog.FileName : null;
	}
}
