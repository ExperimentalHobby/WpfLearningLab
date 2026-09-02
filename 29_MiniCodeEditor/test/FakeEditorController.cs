using MiniCodeEditor.Services;

namespace MiniCodeEditor.Tests;

/// <summary>
/// <see cref="ViewModels.MainViewModel"/> のテスト用に、実際のAvalonEdit TextEditorを使わない
/// <see cref="IEditorController"/>実装。
/// </summary>
public class FakeEditorController : IEditorController
{
	/// <inheritdoc/>
	public string Text { get; set; } = string.Empty;

	/// <summary>最後に<see cref="SetSyntaxHighlighting"/>に渡されたファイルパス。</summary>
	public string? LastSyntaxHighlightingFilePath { get; private set; }

	/// <summary><see cref="SetSyntaxHighlighting"/>が呼ばれた回数。</summary>
	public int SetSyntaxHighlightingCallCount { get; private set; }

	/// <inheritdoc/>
	public void SetSyntaxHighlighting(string? filePath)
	{
		LastSyntaxHighlightingFilePath = filePath;
		SetSyntaxHighlightingCallCount++;
	}
}
