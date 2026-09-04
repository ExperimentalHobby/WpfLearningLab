using MusicPlayer.Services;

namespace MusicPlayer.Tests;

/// <summary>
/// <see cref="ViewModels.MainViewModel"/> のテスト用に使う<see cref="IFolderPicker"/>のフェイク実装。
/// </summary>
public class FakeFolderPicker : IFolderPicker
{
	public string? PathToReturn { get; set; }

	public string? PickFolder() => PathToReturn;
}
