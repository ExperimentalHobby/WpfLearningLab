using PasswordManager.Services;

namespace PasswordManager.Tests;

/// <summary>
/// <see cref="PasswordManager.ViewModels.MainViewModel"/> のテスト用に使う<see cref="IClipboardService"/>のフェイク実装。
/// </summary>
public class FakeClipboardService : IClipboardService
{
	/// <summary>直近に<see cref="SetText"/>で設定されたテキスト(テスト用)。</summary>
	public string? CopiedText { get; private set; }

	/// <summary><see cref="ClearIfUnchanged"/>によりクリアされたかどうか(テスト用)。</summary>
	public bool WasCleared { get; private set; }

	/// <inheritdoc/>
	public void SetText(string text)
	{
		CopiedText = text;
		WasCleared = false;
	}

	/// <inheritdoc/>
	public void ClearIfUnchanged(string expectedText)
	{
		if (CopiedText == expectedText)
		{
			CopiedText = null;
			WasCleared = true;
		}
	}
}
