namespace TaskApiWorkbench.Client.Models;

/// <summary>
/// タスクの作成・更新リクエストのボディ。
/// </summary>
public sealed class TaskItemRequest
{
	/// <summary>タスクのタイトル(必須)。</summary>
	public string Title { get; set; } = string.Empty;

	/// <summary>タスクの説明。</summary>
	public string Description { get; set; } = string.Empty;
}
