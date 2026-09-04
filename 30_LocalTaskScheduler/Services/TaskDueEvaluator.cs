using LocalTaskScheduler.Models;

namespace LocalTaskScheduler.Services;

/// <summary>
/// タスクが実行時刻に到達しているかどうかを判定する純粋ロジック。
/// </summary>
public static class TaskDueEvaluator
{
	/// <summary>
	/// 指定したタスクが、指定した時刻の時点で実行対象かどうかを判定する。
	/// </summary>
	/// <param name="task">判定対象のタスク。</param>
	/// <param name="now">現在時刻。</param>
	public static bool IsDue(ScheduledTask task, DateTime now)
	{
		if (!task.IsEnabled)
		{
			return false;
		}

		return task.ScheduleType switch
		{
			ScheduleType.Once => task.ExecuteAt is { } executeAt && task.LastExecutedAt is null && now >= executeAt,
			ScheduleType.Interval => task.Interval is { } interval
				&& (task.LastExecutedAt is null || now - task.LastExecutedAt >= interval),
			_ => false,
		};
	}
}
