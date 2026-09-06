using MiniCodeEditor.Services;

namespace MiniCodeEditor.Tests;

/// <summary>
/// <see cref="ViewModels.MainViewModel"/> のテスト用に、実際のMessageBoxを表示しない<see cref="IUnsavedChangesPrompt"/>実装。
/// </summary>
public class FakeUnsavedChangesPrompt : IUnsavedChangesPrompt
{
	/// <summary><see cref="Confirm"/>が返す値。</summary>
	public bool? ResultToReturn { get; set; }

	/// <summary><see cref="Confirm"/>が呼ばれた回数。</summary>
	public int CallCount { get; private set; }

	/// <inheritdoc/>
	public bool? Confirm()
	{
		CallCount++;
		return ResultToReturn;
	}
}
