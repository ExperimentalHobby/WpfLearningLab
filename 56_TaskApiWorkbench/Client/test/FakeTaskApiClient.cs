using TaskApiWorkbench.Client.Models;
using TaskApiWorkbench.Client.Services;

namespace TaskApiWorkbench.Client.Tests;

/// <summary>
/// <see cref="ViewModels.MainViewModel"/> のテスト用に、実通信を行わない<see cref="ITaskApiClient"/>実装。
/// 各メソッドの戻り値・例外・呼び出し引数を差し替え/検証可能にする。
/// </summary>
public class FakeTaskApiClient : ITaskApiClient
{
	public IReadOnlyList<TaskItem> TasksResult { get; set; } = [];
	public TaskItem? CreateResult { get; set; }
	public TaskItem? UpdateResult { get; set; }
	public TaskItem? ToggleCompleteResult { get; set; }
	public Exception? ExceptionToThrow { get; set; }

	public (string Title, string Description)? LastCreateCall { get; private set; }
	public (int Id, string Title, string Description)? LastUpdateCall { get; private set; }
	public int? LastToggledId { get; private set; }
	public int? LastDeletedId { get; private set; }

	public Task<IReadOnlyList<TaskItem>> GetTasksAsync()
	{
		if (ExceptionToThrow is not null)
		{
			throw ExceptionToThrow;
		}

		return Task.FromResult(TasksResult);
	}

	public Task<TaskItem> CreateTaskAsync(string title, string description)
	{
		LastCreateCall = (title, description);
		if (ExceptionToThrow is not null)
		{
			throw ExceptionToThrow;
		}

		return Task.FromResult(CreateResult!);
	}

	public Task<TaskItem> UpdateTaskAsync(int id, string title, string description)
	{
		LastUpdateCall = (id, title, description);
		if (ExceptionToThrow is not null)
		{
			throw ExceptionToThrow;
		}

		return Task.FromResult(UpdateResult!);
	}

	public Task<TaskItem> ToggleCompleteAsync(int id)
	{
		LastToggledId = id;
		if (ExceptionToThrow is not null)
		{
			throw ExceptionToThrow;
		}

		return Task.FromResult(ToggleCompleteResult!);
	}

	public Task DeleteTaskAsync(int id)
	{
		LastDeletedId = id;
		if (ExceptionToThrow is not null)
		{
			throw ExceptionToThrow;
		}

		return Task.CompletedTask;
	}
}
