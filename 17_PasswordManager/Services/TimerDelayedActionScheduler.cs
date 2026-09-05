namespace PasswordManager.Services;

/// <summary>
/// <see cref="System.Threading.Timer"/>を使って遅延実行する<see cref="IDelayedActionScheduler"/>実装。
/// </summary>
public class TimerDelayedActionScheduler : IDelayedActionScheduler
{
	/// <inheritdoc/>
	public void Schedule(TimeSpan delay, Action action)
	{
		// タイマー発火まで参照を保持し続ける必要があるため、コールバック内で自身を
		// キャプチャして最後にDisposeする(ローカル変数がGCされてタイマーが
		// 発火しなくなることを防ぐ)。
		System.Threading.Timer? timer = null;
		timer = new System.Threading.Timer(
			_ =>
			{
				action();
				timer?.Dispose();
			},
			null,
			delay,
			Timeout.InfiniteTimeSpan);
	}
}
