using LocalTaskScheduler.Models;
using LocalTaskScheduler.ViewModels;

namespace LocalTaskScheduler.Tests;

/// <summary>
/// <see cref="MainViewModel"/> の単体テスト。
/// </summary>
public class MainViewModelTests
{
	private static readonly DateTime Now = new(2026, 8, 31, 12, 0, 0);

	private static MainViewModel CreateViewModel(FakeToastNotifier? toastNotifier = null) =>
		new(toastNotifier ?? new FakeToastNotifier());

	/// <summary>
	/// パス条件: Once種別でAddTaskCommandを実行すると、Tasksに指定した実行日時のタスクが追加されること
	/// </summary>
	[Fact]
	public void AddTaskCommand_Once種別で実行するとTasksに追加される()
	{
		var viewModel = CreateViewModel();
		viewModel.NewTaskName = "バックアップ";
		viewModel.NewTaskScheduleType = ScheduleType.Once;
		viewModel.NewTaskExecuteAt = "2026-08-31 15:00";

		viewModel.AddTaskCommand.Execute(null);

		var task = Assert.Single(viewModel.Tasks);
		Assert.Equal("バックアップ", task.Name);
		Assert.Equal(ScheduleType.Once, task.ScheduleType);
		Assert.Equal(new DateTime(2026, 8, 31, 15, 0, 0), task.ExecuteAt);
	}

	/// <summary>
	/// パス条件: Interval種別でAddTaskCommandを実行すると、Tasksに指定した分数のタスクが追加されること
	/// </summary>
	[Fact]
	public void AddTaskCommand_Interval種別で実行するとTasksに追加される()
	{
		var viewModel = CreateViewModel();
		viewModel.NewTaskName = "定期チェック";
		viewModel.NewTaskScheduleType = ScheduleType.Interval;
		viewModel.NewTaskIntervalMinutes = "30";

		viewModel.AddTaskCommand.Execute(null);

		var task = Assert.Single(viewModel.Tasks);
		Assert.Equal(ScheduleType.Interval, task.ScheduleType);
		Assert.Equal(TimeSpan.FromMinutes(30), task.Interval);
	}

	/// <summary>
	/// パス条件: AddTaskCommand実行後、入力欄がクリアされること
	/// </summary>
	[Fact]
	public void AddTaskCommand_実行すると入力欄がクリアされる()
	{
		var viewModel = CreateViewModel();
		viewModel.NewTaskName = "バックアップ";
		viewModel.NewTaskScheduleType = ScheduleType.Once;
		viewModel.NewTaskExecuteAt = "2026-08-31 15:00";

		viewModel.AddTaskCommand.Execute(null);

		Assert.Equal(string.Empty, viewModel.NewTaskName);
		Assert.Equal(string.Empty, viewModel.NewTaskExecuteAt);
	}

	/// <summary>
	/// パス条件: タスク名が空欄の場合、AddTaskCommandが実行不可になること
	/// </summary>
	[Fact]
	public void AddTaskCommand_タスク名が空欄の場合CanExecuteがfalseになる()
	{
		var viewModel = CreateViewModel();
		viewModel.NewTaskName = "";
		viewModel.NewTaskScheduleType = ScheduleType.Once;
		viewModel.NewTaskExecuteAt = "2026-08-31 15:00";

		Assert.False(viewModel.AddTaskCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: Once種別で実行日時が不正な文字列の場合、AddTaskCommandが実行不可になること
	/// </summary>
	[Fact]
	public void AddTaskCommand_Once種別で実行日時が不正な場合CanExecuteがfalseになる()
	{
		var viewModel = CreateViewModel();
		viewModel.NewTaskName = "バックアップ";
		viewModel.NewTaskScheduleType = ScheduleType.Once;
		viewModel.NewTaskExecuteAt = "not-a-date";

		Assert.False(viewModel.AddTaskCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: Interval種別で分数が不正な文字列の場合、AddTaskCommandが実行不可になること
	/// </summary>
	[Fact]
	public void AddTaskCommand_Interval種別で分数が不正な場合CanExecuteがfalseになる()
	{
		var viewModel = CreateViewModel();
		viewModel.NewTaskName = "定期チェック";
		viewModel.NewTaskScheduleType = ScheduleType.Interval;
		viewModel.NewTaskIntervalMinutes = "abc";

		Assert.False(viewModel.AddTaskCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: RemoveTaskCommand実行で、Tasksから指定したタスクが削除されること
	/// </summary>
	[Fact]
	public void RemoveTaskCommand_実行するとTasksから指定したタスクが削除される()
	{
		var viewModel = CreateViewModel();
		viewModel.NewTaskName = "バックアップ";
		viewModel.NewTaskScheduleType = ScheduleType.Once;
		viewModel.NewTaskExecuteAt = "2026-08-31 15:00";
		viewModel.AddTaskCommand.Execute(null);
		var task = viewModel.Tasks[0];

		viewModel.RemoveTaskCommand.Execute(task);

		Assert.Empty(viewModel.Tasks);
	}

	/// <summary>
	/// パス条件: CheckDueTasksで期日到来タスクが実行され、ExecutionLogに記録されLastExecutedAtが更新されること
	/// </summary>
	[Fact]
	public void CheckDueTasks_期日到来タスクが実行されExecutionLogに記録される()
	{
		var toastNotifier = new FakeToastNotifier();
		var viewModel = CreateViewModel(toastNotifier);
		viewModel.NewTaskName = "バックアップ";
		viewModel.NewTaskScheduleType = ScheduleType.Once;
		viewModel.NewTaskExecuteAt = "2026-08-31 11:00";
		viewModel.AddTaskCommand.Execute(null);

		viewModel.CheckDueTasks(Now);

		var task = viewModel.Tasks[0];
		Assert.Equal(Now, task.LastExecutedAt);
		Assert.Single(viewModel.ExecutionLog);
		var notification = Assert.Single(toastNotifier.ShownNotifications);
		Assert.Contains("バックアップ", notification.Message);
	}

	/// <summary>
	/// パス条件: CheckDueTasksで期日未到来のタスクは実行されないこと
	/// </summary>
	[Fact]
	public void CheckDueTasks_期日未到来のタスクは実行されない()
	{
		var viewModel = CreateViewModel();
		viewModel.NewTaskName = "バックアップ";
		viewModel.NewTaskScheduleType = ScheduleType.Once;
		viewModel.NewTaskExecuteAt = "2026-08-31 13:00";
		viewModel.AddTaskCommand.Execute(null);

		viewModel.CheckDueTasks(Now);

		Assert.Null(viewModel.Tasks[0].LastExecutedAt);
		Assert.Empty(viewModel.ExecutionLog);
	}

	/// <summary>
	/// パス条件: Once種別タスクは実行後IsEnabledがfalseになり、再度CheckDueTasksを呼んでも再実行されないこと
	/// </summary>
	[Fact]
	public void CheckDueTasks_Once種別タスクは実行後再実行されない()
	{
		var viewModel = CreateViewModel();
		viewModel.NewTaskName = "バックアップ";
		viewModel.NewTaskScheduleType = ScheduleType.Once;
		viewModel.NewTaskExecuteAt = "2026-08-31 11:00";
		viewModel.AddTaskCommand.Execute(null);
		viewModel.CheckDueTasks(Now);

		Assert.False(viewModel.Tasks[0].IsEnabled);
		viewModel.CheckDueTasks(Now.AddMinutes(1));

		Assert.Single(viewModel.ExecutionLog);
	}

	/// <summary>
	/// パス条件: Interval種別タスクは、Interval経過後に再度実行されること
	/// </summary>
	[Fact]
	public void CheckDueTasks_Interval種別タスクはInterval経過後に再度実行される()
	{
		var viewModel = CreateViewModel();
		viewModel.NewTaskName = "定期チェック";
		viewModel.NewTaskScheduleType = ScheduleType.Interval;
		viewModel.NewTaskIntervalMinutes = "10";
		viewModel.AddTaskCommand.Execute(null);
		viewModel.CheckDueTasks(Now);

		Assert.Single(viewModel.ExecutionLog);

		viewModel.CheckDueTasks(Now.AddMinutes(5));
		Assert.Single(viewModel.ExecutionLog);

		viewModel.CheckDueTasks(Now.AddMinutes(10));
		Assert.Equal(2, viewModel.ExecutionLog.Count);
	}
}
