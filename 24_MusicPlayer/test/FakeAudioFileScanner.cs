using MusicPlayer.Services;

namespace MusicPlayer.Tests;

/// <summary>
/// <see cref="ViewModels.MainViewModel"/> のテスト用に使う<see cref="IAudioFileScanner"/>のフェイク実装。
/// </summary>
public class FakeAudioFileScanner : IAudioFileScanner
{
	/// <summary><see cref="GetAudioFilePathsAsync"/>が返す値(テスト用)。</summary>
	public IReadOnlyList<string> PathsToReturn { get; set; } = [];

	/// <inheritdoc/>
	public Task<IReadOnlyList<string>> GetAudioFilePathsAsync(string folderPath) => Task.FromResult(PathsToReturn);
}
