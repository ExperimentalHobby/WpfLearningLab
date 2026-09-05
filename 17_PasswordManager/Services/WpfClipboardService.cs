using System.Windows;

namespace PasswordManager.Services;

/// <summary>
/// WPFの<see cref="Clipboard"/>を使った<see cref="IClipboardService"/>の実装。
/// </summary>
public class WpfClipboardService : IClipboardService
{
	/// <inheritdoc/>
	public void SetText(string text) => Clipboard.SetText(text);

	/// <inheritdoc/>
	public void ClearIfUnchanged(string expectedText)
	{
		try
		{
			if (Clipboard.ContainsText() && Clipboard.GetText() == expectedText)
			{
				Clipboard.Clear();
			}
		}
		catch (System.Runtime.InteropServices.ExternalException)
		{
			// クリップボードが他のプロセスに一時的にロックされている場合等は無視する
			// (自動クリアが失敗しても、次にクリップボードが使われた時点で上書きされるだけで
			// 実害はないため)。
		}
	}
}
