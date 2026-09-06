using System.Windows;

namespace MiniCodeEditor.Services;

/// <summary>
/// <see cref="MessageBox"/>(はい/いいえ/キャンセル)を使う<see cref="IUnsavedChangesPrompt"/>実装。
/// </summary>
public class Win32UnsavedChangesPrompt : IUnsavedChangesPrompt
{
	/// <inheritdoc/>
	public bool? Confirm()
	{
		var result = MessageBox.Show(
			"現在の変更を保存しますか?",
			"簡易コードエディタ",
			MessageBoxButton.YesNoCancel,
			MessageBoxImage.Warning);

		return result switch
		{
			MessageBoxResult.Yes => true,
			MessageBoxResult.No => false,
			_ => null,
		};
	}
}
