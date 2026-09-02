using PaintTool.Services;

namespace PaintTool.Tests;

/// <summary>
/// <see cref="ViewModels.MainViewModel"/> のテスト用に使う<see cref="ISaveFileDialogService"/>のフェイク実装。
/// </summary>
public class FakeSaveFileDialogService : ISaveFileDialogService
{
	public string? PathToReturn { get; set; }

	public string? PromptForSavePath(string defaultExtension, string filter) => PathToReturn;
}
