using ReactiveSearch.Services;

namespace ReactiveSearch.Tests;

/// <summary>
/// <see cref="IScheduler"/> のテスト用Fake。実際には時間経過を待たず、
/// Scheduleの呼び出しと、返した<see cref="IDisposable"/>がキャンセル(Dispose)されたかを記録する。
/// </summary>
public class FakeScheduler : IScheduler
{
    public record ScheduledCall(TimeSpan Delay, Action Action, FakeCancellationToken Token);

    public List<ScheduledCall> Calls { get; } = new();

    public IDisposable Schedule(TimeSpan delay, Action action)
    {
        var token = new FakeCancellationToken();
        Calls.Add(new ScheduledCall(delay, action, token));
        return token;
    }
}

/// <summary>
/// Disposeされたかどうかを記録するだけのテスト用トークン。
/// </summary>
public class FakeCancellationToken : IDisposable
{
    public bool IsDisposed { get; private set; }

    public void Dispose() => IsDisposed = true;
}
