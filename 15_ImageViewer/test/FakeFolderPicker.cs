using ImageViewer.Services;

namespace ImageViewer.Tests;

/// <summary>
/// <see cref="ImageViewer.ViewModels.MainViewModel"/> のテスト用に、実際にダイアログを開かない<see cref="IFolderPicker"/>実装。
/// </summary>
public class FakeFolderPicker : IFolderPicker
{
	/// <summary><see cref="PickFolder"/>が返す値(テスト用)。</summary>
	public string? FolderToReturn { get; set; }

	/// <inheritdoc/>
	public string? PickFolder() => FolderToReturn;
}
