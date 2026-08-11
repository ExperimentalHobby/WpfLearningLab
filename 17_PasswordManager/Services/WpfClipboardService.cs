using System.Windows;

namespace PasswordManager.Services;

/// <summary>
/// WPFの<see cref="Clipboard"/>を使った<see cref="IClipboardService"/>の実装。
/// </summary>
public class WpfClipboardService : IClipboardService
{
	/// <inheritdoc/>
	public void SetText(string text) => Clipboard.SetText(text);
}
