using ReactiveSearch.Services;

namespace ReactiveSearch.Tests;

/// <summary>
/// <see cref="Debouncer"/> のテスト。
/// </summary>
public class DebouncerTests
{
    /// <summary>
    /// パス条件: Triggerを1回だけ呼んだ場合、スケジュールされた分はキャンセルされないこと
    /// </summary>
    [Fact]
    public void Trigger_1回だけ呼んだ場合はキャンセルされない()
    {
        var scheduler = new FakeScheduler();
        var debouncer = new Debouncer(scheduler, TimeSpan.FromMilliseconds(300));

        debouncer.Trigger(() => { });

        Assert.Single(scheduler.Calls);
        Assert.False(scheduler.Calls[0].Token.IsDisposed);
    }

    /// <summary>
    /// パス条件: Triggerを複数回連続で呼ぶと、直前にスケジュールされた分は全てキャンセルされ、
    /// 最後にスケジュールされた分だけが有効(未キャンセル)なまま残ること
    /// </summary>
    [Fact]
    public void Trigger_複数回連続で呼ぶと直前のスケジュールがキャンセルされ最後の1回だけ有効なまま残る()
    {
        var scheduler = new FakeScheduler();
        var debouncer = new Debouncer(scheduler, TimeSpan.FromMilliseconds(300));

        debouncer.Trigger(() => { });
        debouncer.Trigger(() => { });
        debouncer.Trigger(() => { });

        Assert.Equal(3, scheduler.Calls.Count);
        Assert.True(scheduler.Calls[0].Token.IsDisposed);
        Assert.True(scheduler.Calls[1].Token.IsDisposed);
        Assert.False(scheduler.Calls[2].Token.IsDisposed);
    }
}
