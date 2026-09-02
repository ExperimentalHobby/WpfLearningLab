using MusicPlayer.Services;

namespace MusicPlayer.Tests;

/// <summary>
/// <see cref="ViewModels.MainViewModel"/> のテスト用に使う<see cref="IAudioFileScanner"/>のフェイク実装。
/// </summary>
public class FakeAudioFileScanner : IAudioFileScanner
{
	public IReadOnlyList<string> PathsToReturn { get; set; } = [];

	public Task<IReadOnlyList<string>> GetAudioFilePathsAsync(string folderPath) => Task.FromResult(PathsToReturn);
}
