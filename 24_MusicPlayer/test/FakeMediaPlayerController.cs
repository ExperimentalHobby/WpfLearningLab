using MusicPlayer.Services;

namespace MusicPlayer.Tests;

/// <summary>
/// <see cref="ViewModels.MainViewModel"/> のテスト用に使う<see cref="IMediaPlayerController"/>のフェイク実装。
/// </summary>
public class FakeMediaPlayerController : IMediaPlayerController
{
	/// <inheritdoc/>
	public TimeSpan Position { get; set; }

	/// <inheritdoc/>
	public TimeSpan? Duration { get; set; }

	/// <inheritdoc/>
	public event EventHandler? MediaEnded;

	/// <inheritdoc/>
	public event EventHandler? MediaOpened;

	/// <inheritdoc/>
	public event EventHandler<Exception?>? MediaFailed;

	/// <summary>直近に<see cref="Load"/>で読み込まれたパス(テスト用)。</summary>
	public string? LastLoadedPath { get; private set; }

	/// <summary><see cref="Play"/>が呼ばれた回数(テスト用)。</summary>
	public int PlayCallCount { get; private set; }

	/// <summary><see cref="Pause"/>が呼ばれた回数(テスト用)。</summary>
	public int PauseCallCount { get; private set; }

	/// <summary><see cref="Stop"/>が呼ばれた回数(テスト用)。</summary>
	public int StopCallCount { get; private set; }

	/// <inheritdoc/>
	public void Load(string filePath) => LastLoadedPath = filePath;

	/// <inheritdoc/>
	public void Play() => PlayCallCount++;

	/// <inheritdoc/>
	public void Pause() => PauseCallCount++;

	/// <inheritdoc/>
	public void Stop() => StopCallCount++;

	/// <summary><see cref="MediaEnded"/>イベントを発火する(テスト用)。</summary>
	public void RaiseMediaEnded() => MediaEnded?.Invoke(this, EventArgs.Empty);

	/// <summary><see cref="MediaOpened"/>イベントを発火する(テスト用)。</summary>
	public void RaiseMediaOpened() => MediaOpened?.Invoke(this, EventArgs.Empty);

	/// <summary><see cref="MediaFailed"/>イベントを発火する(テスト用)。</summary>
	/// <param name="exception">イベント引数として渡す例外。</param>
	public void RaiseMediaFailed(Exception? exception) => MediaFailed?.Invoke(this, exception);
}
