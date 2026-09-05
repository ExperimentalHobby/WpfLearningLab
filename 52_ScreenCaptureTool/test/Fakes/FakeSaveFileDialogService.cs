using ScreenCaptureTool.Services;

namespace ScreenCaptureTool.Tests.Fakes;

/// <summary>
/// テスト用の<see cref="ISaveFileDialogService"/>フェイク実装。
/// </summary>
public class FakeSaveFileDialogService : ISaveFileDialogService
{
	public string? PathToReturn { get; set; }

	public bool TryGetSavePath(out string? path)
	{
		path = PathToReturn;
		return path is not null;
	}
}
