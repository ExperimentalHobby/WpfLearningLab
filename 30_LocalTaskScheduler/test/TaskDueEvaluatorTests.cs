using LocalTaskScheduler.Models;
using LocalTaskScheduler.Services;

namespace LocalTaskScheduler.Tests;

/// <summary>
/// <see cref="TaskDueEvaluator"/> の単体テスト。
/// </summary>
public class TaskDueEvaluatorTests
{
	private static readonly DateTime Now = new(2026, 8, 31, 12, 0, 0);

	/// <summary>
	/// パス条件: Once種別で実行日時が未到来の場合、falseを返すこと
	/// </summary>
	[Fact]
	public void IsDue_Once種別で実行日時が未到来の場合falseを返す()
	{
		var task = new ScheduledTask("タスク1", ScheduleType.Once, Now.AddMinutes(1), null);

		Assert.False(TaskDueEvaluator.IsDue(task, Now));
	}

	/// <summary>
	/// パス条件: Once種別で実行日時が到来している場合、trueを返すこと
	/// </summary>
	[Fact]
	public void IsDue_Once種別で実行日時が到来している場合trueを返す()
	{
		var task = new ScheduledTask("タスク1", ScheduleType.Once, Now.AddMinutes(-1), null);

		Assert.True(TaskDueEvaluator.IsDue(task, Now));
	}

	/// <summary>
	/// パス条件: Once種別で既に実行済み(LastExecutedAt設定済み)の場合、falseを返すこと
	/// </summary>
	[Fact]
	public void IsDue_Once種別で実行済みの場合falseを返す()
	{
		var task = new ScheduledTask("タスク1", ScheduleType.Once, Now.AddMinutes(-1), null)
		{
			LastExecutedAt = Now.AddMinutes(-1),
		};

		Assert.False(TaskDueEvaluator.IsDue(task, Now));
	}

	/// <summary>
	/// パス条件: Interval種別で未実行の場合、trueを返すこと(初回は即座に実行対象になる)
	/// </summary>
	[Fact]
	public void IsDue_Interval種別で未実行の場合trueを返す()
	{
		var task = new ScheduledTask("タスク1", ScheduleType.Interval, null, TimeSpan.FromMinutes(10));

		Assert.True(TaskDueEvaluator.IsDue(task, Now));
	}

	/// <summary>
	/// パス条件: Interval種別で前回実行から間隔未経過の場合、falseを返すこと
	/// </summary>
	[Fact]
	public void IsDue_Interval種別で間隔未経過の場合falseを返す()
	{
		var task = new ScheduledTask("タスク1", ScheduleType.Interval, null, TimeSpan.FromMinutes(10))
		{
			LastExecutedAt = Now.AddMinutes(-5),
		};

		Assert.False(TaskDueEvaluator.IsDue(task, Now));
	}

	/// <summary>
	/// パス条件: Interval種別で前回実行から間隔以上経過した場合、trueを返すこと
	/// </summary>
	[Fact]
	public void IsDue_Interval種別で間隔以上経過した場合trueを返す()
	{
		var task = new ScheduledTask("タスク1", ScheduleType.Interval, null, TimeSpan.FromMinutes(10))
		{
			LastExecutedAt = Now.AddMinutes(-10),
		};

		Assert.True(TaskDueEvaluator.IsDue(task, Now));
	}

	/// <summary>
	/// パス条件: 無効化(IsEnabled=false)されたタスクは、期日到来していてもfalseを返すこと
	/// </summary>
	[Fact]
	public void IsDue_無効化されたタスクはfalseを返す()
	{
		var task = new ScheduledTask("タスク1", ScheduleType.Once, Now.AddMinutes(-1), null)
		{
			IsEnabled = false,
		};

		Assert.False(TaskDueEvaluator.IsDue(task, Now));
	}
}
