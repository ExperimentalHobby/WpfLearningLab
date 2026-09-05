using PasswordManager.Services;

namespace PasswordManager.Tests;

/// <summary>
/// <see cref="MainViewModel"/> のテスト用に使う<see cref="IClipboardService"/>のフェイク実装。
/// </summary>
public class FakeClipboardService : IClipboardService
{
	public string? CopiedText { get; private set; }
	public bool WasCleared { get; private set; }

	public void SetText(string text)
	{
		CopiedText = text;
		WasCleared = false;
	}

	public void ClearIfUnchanged(string expectedText)
	{
		if (CopiedText == expectedText)
		{
			CopiedText = null;
			WasCleared = true;
		}
	}
}
