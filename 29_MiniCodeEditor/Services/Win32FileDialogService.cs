using Microsoft.Win32;

namespace MiniCodeEditor.Services;

/// <summary>
/// <see cref="Microsoft.Win32.OpenFileDialog"/>/<see cref="Microsoft.Win32.SaveFileDialog"/>を使う
/// <see cref="IFileDialogService"/>実装。
/// </summary>
public class Win32FileDialogService : IFileDialogService
{
	private const string FileFilter = "すべてのファイル (*.*)|*.*|C# (*.cs)|*.cs|XML (*.xml)|*.xml|テキスト (*.txt)|*.txt";

	/// <inheritdoc/>
	public string? ShowOpenDialog()
	{
		var dialog = new OpenFileDialog { Filter = FileFilter };
		return dialog.ShowDialog() == true ? dialog.FileName : null;
	}

	/// <inheritdoc/>
	public string? ShowSaveDialog(string? suggestedFileName)
	{
		var dialog = new SaveFileDialog { Filter = FileFilter, FileName = suggestedFileName ?? string.Empty };
		return dialog.ShowDialog() == true ? dialog.FileName : null;
	}
}
