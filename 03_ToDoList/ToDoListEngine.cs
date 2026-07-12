namespace ToDoList;

/// <summary>
/// ToDoタスクの追加・削除・完了切り替え・編集(CRUD)を行うエンジン。
/// </summary>
public class ToDoListEngine
{
	private readonly List<ToDoTask> _tasks = [];
	private int _nextId = 1;

	/// <summary>
	/// 現在登録されているタスクの一覧。
	/// </summary>
	public IReadOnlyList<ToDoTask> Tasks => _tasks;

	/// <summary>
	/// 新しいタスクを追加する。
	/// </summary>
	/// <param name="title">タスクのタイトル。</param>
	public void AddTask(string title)
	{
		var trimmed = title.Trim();
		if (trimmed.Length == 0)
		{
			return;
		}

		_tasks.Add(new ToDoTask { Id = _nextId++, Title = trimmed });
	}

	/// <summary>
	/// 指定したIdのタスクを削除する。
	/// </summary>
	/// <param name="id">削除するタスクのId。</param>
	/// <returns>削除できた場合は true、該当するタスクがない場合は false。</returns>
	public bool RemoveTask(int id)
	{
		return _tasks.RemoveAll(t => t.Id == id) > 0;
	}

	/// <summary>
	/// 指定したIdのタスクの完了/未完了状態を反転する。
	/// </summary>
	/// <param name="id">対象タスクのId。</param>
	/// <returns>切り替えできた場合は true、該当するタスクがない場合は false。</returns>
	public bool ToggleDone(int id)
	{
		var task = FindTask(id);
		if (task is null)
		{
			return false;
		}

		task.IsDone = !task.IsDone;
		return true;
	}

	/// <summary>
	/// 指定したIdのタスクのタイトルを変更する。
	/// </summary>
	/// <param name="id">対象タスクのId。</param>
	/// <param name="newTitle">新しいタイトル。空白のみの場合は変更しない。</param>
	/// <returns>変更できた場合は true、該当するタスクがない、または新タイトルが空白のみの場合は false。</returns>
	public bool EditTitle(int id, string newTitle)
	{
		var trimmed = newTitle.Trim();
		if (trimmed.Length == 0)
		{
			return false;
		}

		var task = FindTask(id);
		if (task is null)
		{
			return false;
		}

		task.Title = trimmed;
		return true;
	}

	private ToDoTask? FindTask(int id) => _tasks.FirstOrDefault(t => t.Id == id);
}
