using MusicPlayer.Services;

namespace MusicPlayer.Tests;

/// <summary>
/// <see cref="ViewModels.MainViewModel"/> のテスト用に使う<see cref="IFolderPicker"/>のフェイク実装。
/// </summary>
public class FakeFolderPicker : IFolderPicker
{
	/// <summary><see cref="PickFolder"/>が返す値(テスト用)。</summary>
	public string? PathToReturn { get; set; }

	/// <inheritdoc/>
	public string? PickFolder() => PathToReturn;
}
