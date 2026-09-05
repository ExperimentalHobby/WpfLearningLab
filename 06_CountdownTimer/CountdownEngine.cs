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
	/// 時・分・秒の入力値から初期時間を設定する。<see cref="TimeSpan"/>の表現範囲を超える
	/// 巨大な値が渡された場合は例外を投げず false を返す(入力欄の値をそのまま
	/// <c>new TimeSpan(hours, minutes, seconds)</c> に渡すと ArgumentOutOfRangeException で
	/// クラッシュするため)。
	/// </summary>
	/// <param name="hours">時間。</param>
	/// <param name="minutes">分。</param>
	/// <param name="seconds">秒。</param>
	/// <returns>設定できた場合は true、値が範囲外で設定できなかった場合は false。</returns>
	public bool TrySetInitialTimeFromParts(int hours, int minutes, int seconds)
	{
		TimeSpan time;
		try
		{
			time = new TimeSpan(hours, minutes, seconds);
		}
		catch (ArgumentOutOfRangeException)
		{
			return false;
		}

		SetInitialTime(time);
		return true;
	}

	/// <summary>
	/// 残り時間を "時:分:秒" 形式の文字列にフォーマットする。時間部分は
	/// <see cref="TimeSpan.TotalHours"/> を整数に切り捨てた値を使うため、24時間以上でも
	/// (<c>ToString(@"hh\:mm\:ss")</c> のように日の繰り上がりで表示が壊れることなく)
	/// 正しく表示できる。
	/// </summary>
	public string FormatRemainingTime()
	{
		var totalHours = (int)RemainingTime.TotalHours;
		return $"{totalHours:00}:{RemainingTime.Minutes:00}:{RemainingTime.Seconds:00}";
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
