using TaskApiWorkbench.Client.Models;
using TaskApiWorkbench.Client.Services;

namespace TaskApiWorkbench.Client.Tests;

/// <summary>
/// <see cref="ViewModels.MainViewModel"/> のテスト用に、実通信を行わない<see cref="ITaskApiClient"/>実装。
/// 各メソッドの戻り値・例外・呼び出し引数を差し替え/検証可能にする。
/// </summary>
public class FakeTaskApiClient : ITaskApiClient
{
	/// <summary><see cref="GetTasksAsync"/>が返す値(テスト用)。</summary>
	public IReadOnlyList<TaskItem> TasksResult { get; set; } = [];

	/// <summary><see cref="CreateTaskAsync"/>が返す値(テスト用)。</summary>
	public TaskItem? CreateResult { get; set; }

	/// <summary><see cref="UpdateTaskAsync"/>が返す値(テスト用)。</summary>
	public TaskItem? UpdateResult { get; set; }

	/// <summary><see cref="ToggleCompleteAsync"/>が返す値(テスト用)。</summary>
	public TaskItem? ToggleCompleteResult { get; set; }

	/// <summary>設定すると各メソッド呼び出し時にこの例外をスローする(テスト用)。</summary>
	public Exception? ExceptionToThrow { get; set; }

	/// <summary>直近の<see cref="CreateTaskAsync"/>呼び出し引数(テスト用)。</summary>
	public (string Title, string Description)? LastCreateCall { get; private set; }

	/// <summary>直近の<see cref="UpdateTaskAsync"/>呼び出し引数(テスト用)。</summary>
	public (int Id, string Title, string Description)? LastUpdateCall { get; private set; }

	/// <summary>直近の<see cref="ToggleCompleteAsync"/>呼び出し引数(テスト用)。</summary>
	public int? LastToggledId { get; private set; }

	/// <summary>直近の<see cref="DeleteTaskAsync"/>呼び出し引数(テスト用)。</summary>
	public int? LastDeletedId { get; private set; }

	/// <inheritdoc/>
	public Task<IReadOnlyList<TaskItem>> GetTasksAsync()
	{
		if (ExceptionToThrow is not null)
		{
			throw ExceptionToThrow;
		}

		return Task.FromResult(TasksResult);
	}

	/// <inheritdoc/>
	public Task<TaskItem> CreateTaskAsync(string title, string description)
	{
		LastCreateCall = (title, description);
		if (ExceptionToThrow is not null)
		{
			throw ExceptionToThrow;
		}

		return Task.FromResult(CreateResult!);
	}

	/// <inheritdoc/>
	public Task<TaskItem> UpdateTaskAsync(int id, string title, string description)
	{
		LastUpdateCall = (id, title, description);
		if (ExceptionToThrow is not null)
		{
			throw ExceptionToThrow;
		}

		return Task.FromResult(UpdateResult!);
	}

	/// <inheritdoc/>
	public Task<TaskItem> ToggleCompleteAsync(int id)
	{
		LastToggledId = id;
		if (ExceptionToThrow is not null)
		{
			throw ExceptionToThrow;
		}

		return Task.FromResult(ToggleCompleteResult!);
	}

	/// <inheritdoc/>
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
