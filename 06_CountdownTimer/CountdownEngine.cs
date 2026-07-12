namespace CountdownTimer;

/// <summary>
/// カウントダウンタイマーの状態(<see cref="CountdownState"/>)と残り時間を管理する状態機械。
/// <see cref="System.Windows.Threading.DispatcherTimer"/> には依存せず、1秒ごとの <see cref="Tick"/> 呼び出しを外部(UI層)に委ねる。
/// </summary>
public class CountdownEngine
{
	/// <summary>
	/// 現在の状態。
	/// </summary>
	public CountdownState State { get; private set; } = CountdownState.Stopped;

	/// <summary>
	/// 現在の残り時間。
	/// </summary>
	public TimeSpan RemainingTime { get; private set; } = TimeSpan.Zero;

	private TimeSpan _initialTime = TimeSpan.Zero;

	/// <summary>
	/// カウントダウンの初期値を設定する。Stopped状態のときのみ有効。
	/// </summary>
	/// <param name="time">カウントダウン開始時間。</param>
	public void SetInitialTime(TimeSpan time)
	{
		if (State != CountdownState.Stopped)
		{
			return;
		}

		_initialTime = time;
		RemainingTime = time;
	}

	/// <summary>
	/// カウントダウンを開始する。Stopped/Pausedかつ残り時間が0より大きいときのみRunningになる。
	/// 一時停止からの再開もこのメソッドで行う。
	/// </summary>
	public void Start()
	{
		if ((State != CountdownState.Stopped && State != CountdownState.Paused) || RemainingTime <= TimeSpan.Zero)
		{
			return;
		}

		State = CountdownState.Running;
	}

	/// <summary>
	/// 1秒分カウントダウンを進める。Running中でなければ何もしない。
	/// </summary>
	/// <returns>この呼び出しでちょうど残り時間が0に達した場合は true。</returns>
	public bool Tick()
	{
		if (State != CountdownState.Running)
		{
			return false;
		}

		RemainingTime -= TimeSpan.FromSeconds(1);

		if (RemainingTime <= TimeSpan.Zero)
		{
			RemainingTime = TimeSpan.Zero;
			State = CountdownState.Completed;
			return true;
		}

		return false;
	}

	/// <summary>
	/// カウントダウンを一時停止する。Running中でなければ何もしない。
	/// </summary>
	public void Pause()
	{
		if (State != CountdownState.Running)
		{
			return;
		}

		State = CountdownState.Paused;
	}

	/// <summary>
	/// 残り時間を初期値に戻し、状態をStoppedにリセットする。
	/// </summary>
	public void Reset()
	{
		RemainingTime = _initialTime;
		State = CountdownState.Stopped;
	}
}
