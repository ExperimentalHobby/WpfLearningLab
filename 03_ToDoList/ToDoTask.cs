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
	/// タスクのタイトル。<see cref="ToDoListEngine"/>が生成時に必ず設定する。
	/// </summary>
	/// <remarks>
	/// 外部コードが <see cref="ToDoListEngine"/> を経由せず直接書き換えられないよう、
	/// setterは同一アセンブリ内(<see cref="ToDoListEngine"/>)からのみ許可する。
	/// C#の <c>required</c> メンバーはコンテナと同じ可視性のsetterを要求するため
	/// (setterをinternalにすると <c>required</c> は付けられない)、代わりに
	/// <see cref="ToDoListEngine"/> が生成直後に必ず設定する運用で担保する。
	/// </remarks>
	public string Title { get; internal set; } = string.Empty;

	/// <summary>
	/// タスクが完了済みかどうか。
	/// </summary>
	/// <remarks>
	/// 外部コードが <see cref="ToDoListEngine"/> を経由せず直接書き換えられないよう、
	/// setterは同一アセンブリ内(<see cref="ToDoListEngine"/>)からのみ許可する。
	/// </remarks>
	public bool IsDone { get; internal set; }
}
