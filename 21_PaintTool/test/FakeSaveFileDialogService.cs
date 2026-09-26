using PaintTool.Services;

namespace PaintTool.Tests;

/// <summary>
/// <see cref="ViewModels.MainViewModel"/> のテスト用に使う<see cref="ISaveFileDialogService"/>のフェイク実装。
/// </summary>
public class FakeSaveFileDialogService : ISaveFileDialogService
{
	/// <summary><see cref="PromptForSavePath"/>が返す値(テスト用)。</summary>
	public string? PathToReturn { get; set; }

	/// <inheritdoc/>
	public string? PromptForSavePath(string defaultExtension, string filter) => PathToReturn;
}
