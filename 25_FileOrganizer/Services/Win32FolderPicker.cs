using Microsoft.Win32;

namespace FileOrganizer.Services;

/// <summary>
/// <see cref="OpenFolderDialog"/>(.NET 8+のWPF標準フォルダ選択ダイアログ)を使う<see cref="IFolderPicker"/>実装。
/// </summary>
public class Win32FolderPicker : IFolderPicker
{
	/// <inheritdoc/>
	public string? PickFolder()
	{
		var dialog = new OpenFolderDialog();
		return dialog.ShowDialog() == true ? dialog.FolderName : null;
	}
}
