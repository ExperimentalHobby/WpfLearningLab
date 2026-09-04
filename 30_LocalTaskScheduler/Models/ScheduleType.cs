namespace LocalTaskScheduler.Models;

/// <summary>
/// タスクの実行スケジュール種別。
/// </summary>
public enum ScheduleType
{
	/// <summary>指定した日時に1回だけ実行する。</summary>
	Once,

	/// <summary>一定間隔で繰り返し実行する。</summary>
	Interval,
}
