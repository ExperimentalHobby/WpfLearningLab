using FileOrganizer.Services;

namespace FileOrganizer.Tests;

/// <summary>
/// <see cref="ViewModels.MainViewModel"/> のテスト用に、実際のダイアログを開かない<see cref="IFolderPicker"/>実装。
/// </summary>
public class FakeFolderPicker : IFolderPicker
{
	/// <summary><see cref="PickFolder"/>が返す値(未設定時は<see langword="null"/>、キャンセル相当)。</summary>
	public string? FolderToReturn { get; set; }

	/// <inheritdoc/>
	public string? PickFolder() => FolderToReturn;
}
