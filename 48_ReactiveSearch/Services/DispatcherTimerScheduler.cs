using System.Windows.Threading;

namespace ReactiveSearch.Services;

/// <summary>
/// <see cref="DispatcherTimer"/> を使った <see cref="IScheduler"/> の実装。
/// UIスレッドのディスパッチャ上でコールバックが実行される。
/// </summary>
public class DispatcherTimerScheduler : IScheduler
{
    /// <inheritdoc />
    public IDisposable Schedule(TimeSpan delay, Action action)
    {
        var timer = new DispatcherTimer { Interval = delay };
        EventHandler? handler = null;
        handler = (_, _) =>
        {
            timer.Stop();
            timer.Tick -= handler;
            action();
        };
        timer.Tick += handler;
        timer.Start();

        return new TimerCancellation(timer, handler);
    }

    private sealed class TimerCancellation : IDisposable
    {
        private readonly DispatcherTimer _timer;
        private readonly EventHandler _handler;

        public TimerCancellation(DispatcherTimer timer, EventHandler handler)
        {
            _timer = timer;
            _handler = handler;
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Tick -= _handler;
        }
    }
}
