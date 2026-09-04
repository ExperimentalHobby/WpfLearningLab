using System.Threading;

namespace LocalTaskScheduler.Services;

/// <summary>
/// 実の<see cref="System.Threading.Timer"/>を使う<see cref="IBackgroundTicker"/>実装。
/// </summary>
public class ThreadingTimerTicker : IBackgroundTicker
{
	private Timer? _timer;

	/// <inheritdoc/>
	public event Action<DateTime>? Ticked;

	/// <inheritdoc/>
	public void Start(TimeSpan interval)
	{
		Stop();
		_timer = new Timer(_ => Ticked?.Invoke(DateTime.Now), null, interval, interval);
	}

	/// <inheritdoc/>
	public void Stop()
	{
		_timer?.Dispose();
		_timer = null;
	}

	/// <inheritdoc/>
	public void Dispose() => Stop();
}
