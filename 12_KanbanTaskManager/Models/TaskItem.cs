namespace KanbanTaskManager.Models;

/// <summary>
/// カンバンボード上のタスクの状態(カラム)。
/// 名称を <c>KanbanStatus</c> とし、<see cref="System.Threading.Tasks.TaskStatus"/> との名前衝突を避けている。
/// </summary>
public enum KanbanStatus
{
	/// <summary>未着手。</summary>
	Todo,

	/// <summary>対応中。</summary>
	InProgress,

	/// <summary>完了。</summary>
	Done,
}

/// <summary>
/// カンバンボード上の1つのタスク。
/// </summary>
public class TaskItem
{
	/// <summary>タスクを一意に識別するID。</summary>
	public Guid Id { get; init; } = Guid.NewGuid();

	/// <summary>タスクのタイトル。</summary>
	public required string Title { get; set; }

	/// <summary>現在の状態(所属カラム)。</summary>
	public required KanbanStatus Status { get; set; }
}
