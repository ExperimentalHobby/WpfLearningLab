using LocalTaskScheduler.Models;
using LocalTaskScheduler.Services;

namespace LocalTaskScheduler.Tests;

/// <summary>
/// <see cref="ViewModels.MainViewModel"/> のテスト用に、実際のファイルI/Oを行わない<see cref="ITaskRepository"/>実装。
/// </summary>
public class FakeTaskRepository : ITaskRepository
{
	/// <summary><see cref="Load"/>が返すタスク一覧。</summary>
	public IReadOnlyList<ScheduledTask> TasksToReturn { get; set; } = [];

	/// <summary>最後に<see cref="Save"/>に渡されたタスク一覧。</summary>
	public IReadOnlyList<ScheduledTask>? LastSavedTasks { get; private set; }

	/// <summary><see cref="Save"/>が呼ばれた回数。</summary>
	public int SaveCallCount { get; private set; }

	/// <inheritdoc/>
	public IReadOnlyList<ScheduledTask> Load() => TasksToReturn;

	/// <inheritdoc/>
	public void Save(IReadOnlyList<ScheduledTask> tasks)
	{
		LastSavedTasks = tasks;
		SaveCallCount++;
	}
}
