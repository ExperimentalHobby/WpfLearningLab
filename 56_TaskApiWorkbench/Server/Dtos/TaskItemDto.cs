namespace TaskApiWorkbench.Server.Dtos;

/// <summary>
/// タスク取得APIのレスポンスボディ。
/// </summary>
public sealed class TaskItemDto
{
	/// <summary>タスクのID。</summary>
	public int Id { get; set; }

	/// <summary>タスクのタイトル。</summary>
	public string Title { get; set; } = string.Empty;

	/// <summary>タスクの説明。</summary>
	public string Description { get; set; } = string.Empty;

	/// <summary>完了しているかどうか。</summary>
	public bool IsCompleted { get; set; }
}
