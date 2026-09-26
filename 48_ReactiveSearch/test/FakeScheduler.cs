using ReactiveSearch.Services;

namespace ReactiveSearch.Tests;

/// <summary>
/// <see cref="IScheduler"/> のテスト用Fake。実際には時間経過を待たず、
/// Scheduleの呼び出しと、返した<see cref="IDisposable"/>がキャンセル(Dispose)されたかを記録する。
/// </summary>
public class FakeScheduler : IScheduler
{
    /// <summary><see cref="Schedule"/>呼び出し1件分の記録(テスト用)。</summary>
    /// <param name="Delay">要求された遅延時間。</param>
    /// <param name="Action">スケジュールされたアクション。</param>
    /// <param name="Token">キャンセル(Dispose)状態を記録するトークン。</param>
    public record ScheduledCall(TimeSpan Delay, Action Action, FakeCancellationToken Token);

    /// <summary><see cref="Schedule"/>が呼ばれるたびに記録される呼び出し履歴(テスト用)。</summary>
    public List<ScheduledCall> Calls { get; } = new();

    /// <inheritdoc/>
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
    /// <summary><see cref="Dispose"/>が呼ばれたかどうか(テスト用)。</summary>
    public bool IsDisposed { get; private set; }

    /// <inheritdoc/>
    public void Dispose() => IsDisposed = true;
}
