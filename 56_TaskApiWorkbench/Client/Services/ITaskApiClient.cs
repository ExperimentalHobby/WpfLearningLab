using TaskApiWorkbench.Client.Models;

namespace TaskApiWorkbench.Client.Services;

/// <summary>
/// タスク管理サーバーとの通信を担うクライアントの抽象。
/// </summary>
public interface ITaskApiClient
{
	/// <summary>
	/// タスクの一覧を取得する。
	/// </summary>
	Task<IReadOnlyList<TaskItem>> GetTasksAsync();

	/// <summary>
	/// タスクを新規作成する。
	/// </summary>
	Task<TaskItem> CreateTaskAsync(string title, string description);

	/// <summary>
	/// 指定IDのタスクを更新する。
	/// </summary>
	Task<TaskItem> UpdateTaskAsync(int id, string title, string description);

	/// <summary>
	/// 指定IDのタスクの完了状態をトグルする。
	/// </summary>
	Task<TaskItem> ToggleCompleteAsync(int id);

	/// <summary>
	/// 指定IDのタスクを削除する。
	/// </summary>
	Task DeleteTaskAsync(int id);
}
