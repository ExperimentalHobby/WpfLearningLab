namespace ToDoList;

/// <summary>
/// ToDoリストの1タスクを表すモデル。
/// </summary>
public class ToDoTask
{
	/// <summary>
	/// タスクの一意なID。
	/// </summary>
	public required int Id { get; init; }

	/// <summary>
	/// タスクのタイトル。
	/// </summary>
	public required string Title { get; set; }

	/// <summary>
	/// タスクが完了済みかどうか。
	/// </summary>
	public bool IsDone { get; set; }
}
