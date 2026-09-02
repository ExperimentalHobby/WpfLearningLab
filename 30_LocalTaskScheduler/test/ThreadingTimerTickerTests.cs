using LocalTaskScheduler.Services;

namespace LocalTaskScheduler.Tests;

/// <summary>
/// <see cref="ThreadingTimerTicker"/> の単体テスト。実の<see cref="System.Threading.Timer"/>に対して検証する。
/// </summary>
public class ThreadingTimerTickerTests
{
	/// <summary>
	/// パス条件: Start後、指定した間隔でTickedイベントが発火すること
	/// </summary>
	[Fact]
	public async Task Start_指定した間隔でTickedが発火する()
	{
		using var ticker = new ThreadingTimerTicker();
		var tcs = new TaskCompletionSource<DateTime>();
		ticker.Ticked += now => tcs.TrySetResult(now);

		ticker.Start(TimeSpan.FromMilliseconds(200));
		var completed = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(5)));

		Assert.Same(tcs.Task, completed);
	}

	/// <summary>
	/// パス条件: Stop後は、それ以降Tickedが発火しないこと
	/// </summary>
	[Fact]
	public async Task Stop_呼び出した後はTickedが発火しない()
	{
		using var ticker = new ThreadingTimerTicker();
		var tickCount = 0;
		ticker.Ticked += _ => Interlocked.Increment(ref tickCount);
		ticker.Start(TimeSpan.FromMilliseconds(100));
		await Task.Delay(TimeSpan.FromMilliseconds(150));

		ticker.Stop();
		var countAfterStop = tickCount;
		await Task.Delay(TimeSpan.FromMilliseconds(300));

		Assert.Equal(countAfterStop, tickCount);
	}
}
