namespace ReactiveSearch.Services;

/// <summary>
/// 短時間に連続して <see cref="Trigger"/> が呼ばれた場合、直前にスケジュールした分を
/// キャンセルして新たにスケジュールし直すことで、最後の呼び出しだけを遅延実行する(debounce)。
/// </summary>
public class Debouncer
{
    private readonly IScheduler _scheduler;
    private readonly TimeSpan _delay;
    private IDisposable? _pending;

    public Debouncer(IScheduler scheduler, TimeSpan delay)
    {
        _scheduler = scheduler;
        _delay = delay;
    }

    /// <summary>
    /// アクションの実行をdebounce付きでスケジュールする。
    /// </summary>
    public void Trigger(Action action)
    {
        _pending?.Dispose();
        _pending = _scheduler.Schedule(_delay, action);
    }
}
