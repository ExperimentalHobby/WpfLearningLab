using MusicPlayer.Services;

namespace MusicPlayer.Tests;

/// <summary>
/// <see cref="ViewModels.MainViewModel"/> のテスト用に使う<see cref="IMediaPlayerController"/>のフェイク実装。
/// </summary>
public class FakeMediaPlayerController : IMediaPlayerController
{
	public TimeSpan Position { get; set; }

	public TimeSpan? Duration { get; set; }

	public event EventHandler? MediaEnded;

	public event EventHandler? MediaOpened;

	public event EventHandler<Exception?>? MediaFailed;

	public string? LastLoadedPath { get; private set; }
	public int PlayCallCount { get; private set; }
	public int PauseCallCount { get; private set; }
	public int StopCallCount { get; private set; }

	public void Load(string filePath) => LastLoadedPath = filePath;

	public void Play() => PlayCallCount++;

	public void Pause() => PauseCallCount++;

	public void Stop() => StopCallCount++;

	public void RaiseMediaEnded() => MediaEnded?.Invoke(this, EventArgs.Empty);

	public void RaiseMediaOpened() => MediaOpened?.Invoke(this, EventArgs.Empty);

	public void RaiseMediaFailed(Exception? exception) => MediaFailed?.Invoke(this, exception);
}
