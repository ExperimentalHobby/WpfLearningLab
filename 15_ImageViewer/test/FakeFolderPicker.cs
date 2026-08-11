using ImageViewer.Services;

namespace ImageViewer.Tests;

/// <summary>
/// <see cref="MainViewModel"/> のテスト用に、実際にダイアログを開かない<see cref="IFolderPicker"/>実装。
/// </summary>
public class FakeFolderPicker : IFolderPicker
{
	public string? FolderToReturn { get; set; }

	public string? PickFolder() => FolderToReturn;
}
