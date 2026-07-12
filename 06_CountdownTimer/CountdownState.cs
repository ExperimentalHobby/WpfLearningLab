namespace CountdownTimer;

/// <summary>
/// カウントダウンタイマーの状態。
/// </summary>
public enum CountdownState
{
	/// <summary>
	/// 未開始、またはリセット直後。入力欄を編集できる。
	/// </summary>
	Stopped,

	/// <summary>
	/// カウントダウン実行中。
	/// </summary>
	Running,

	/// <summary>
	/// 一時停止中。
	/// </summary>
	Paused,

	/// <summary>
	/// 残り時間が0になり完了した状態。
	/// </summary>
	Completed,
}
