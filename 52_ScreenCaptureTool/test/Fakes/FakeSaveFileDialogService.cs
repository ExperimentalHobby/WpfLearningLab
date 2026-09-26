using ScreenCaptureTool.Services;

namespace ScreenCaptureTool.Tests.Fakes;

/// <summary>
/// テスト用の<see cref="ISaveFileDialogService"/>フェイク実装。
/// </summary>
public class FakeSaveFileDialogService : ISaveFileDialogService
{
	/// <summary><see cref="TryGetSavePath"/>が返す値(テスト用)。</summary>
	public string? PathToReturn { get; set; }

	/// <inheritdoc/>
	public bool TryGetSavePath(out string? path)
	{
		path = PathToReturn;
		return path is not null;
	}
}
