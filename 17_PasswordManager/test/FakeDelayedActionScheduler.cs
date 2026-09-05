using PasswordManager.Services;

namespace PasswordManager.Tests;

/// <summary>
/// <see cref="MainViewModel"/> のテスト用に、実時間を待たずに検証できる
/// <see cref="IDelayedActionScheduler"/>のフェイク実装。スケジュールされたコールバックを
/// 記録するだけで、<see cref="RunAll"/>を呼ぶまで実行しない。
/// </summary>
public class FakeDelayedActionScheduler : IDelayedActionScheduler
{
	private readonly List<(TimeSpan Delay, Action Action)> _scheduled = [];

	public IReadOnlyList<TimeSpan> ScheduledDelays => _scheduled.Select(s => s.Delay).ToList();

	public void Schedule(TimeSpan delay, Action action) => _scheduled.Add((delay, action));

	/// <summary>スケジュール済みの全コールバックを今すぐ実行する(実時間の経過をシミュレートする)。</summary>
	public void RunAll()
	{
		var toRun = _scheduled.ToList();
		_scheduled.Clear();
		foreach (var (_, action) in toRun)
		{
			action();
		}
	}
}
