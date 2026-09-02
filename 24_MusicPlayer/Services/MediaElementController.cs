using System.Windows.Controls;

namespace MusicPlayer.Services;

/// <summary>
/// 実際の<see cref="MediaElement"/>をラップする<see cref="IMediaPlayerController"/>実装。
/// </summary>
public class MediaElementController : IMediaPlayerController
{
	private readonly MediaElement _mediaElement;

	/// <summary>
	/// コントローラーを初期化する。
	/// </summary>
	/// <param name="mediaElement">操作対象のMediaElement。<see cref="MediaElement.LoadedBehavior"/>は<c>Manual</c>にしておくこと。</param>
	public MediaElementController(MediaElement mediaElement)
	{
		_mediaElement = mediaElement;
		_mediaElement.MediaEnded += (_, _) => MediaEnded?.Invoke(this, EventArgs.Empty);
		_mediaElement.MediaOpened += (_, _) => MediaOpened?.Invoke(this, EventArgs.Empty);
	}

	/// <inheritdoc/>
	public TimeSpan Position
	{
		get => _mediaElement.Position;
		set => _mediaElement.Position = value;
	}

	/// <inheritdoc/>
	public TimeSpan? Duration => _mediaElement.NaturalDuration.HasTimeSpan ? _mediaElement.NaturalDuration.TimeSpan : null;

	/// <inheritdoc/>
	public event EventHandler? MediaEnded;

	/// <inheritdoc/>
	public event EventHandler? MediaOpened;

	/// <inheritdoc/>
	public void Load(string filePath)
	{
		_mediaElement.Source = new Uri(filePath);
	}

	/// <inheritdoc/>
	public void Play() => _mediaElement.Play();

	/// <inheritdoc/>
	public void Pause() => _mediaElement.Pause();

	/// <inheritdoc/>
	public void Stop() => _mediaElement.Stop();
}
