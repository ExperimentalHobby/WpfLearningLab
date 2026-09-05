using KanbanTaskManager.Models;

namespace KanbanTaskManager.ViewModels;

/// <summary>
/// ドラッグ&ドロップで発生した「タスクを指定カラムへ移動する」要求。
/// <see cref="Behaviors.DragDropBehavior"/> から <see cref="MainViewModel.MoveTaskCommand"/> へ渡される。
/// </summary>
/// <param name="Task">移動対象のタスク。</param>
/// <param name="TargetStatus">移動先の状態(カラム)。</param>
public record MoveTaskRequest(TaskItem Task, KanbanStatus TargetStatus);

/// <summary>
/// カンバンボード全体のViewModel。Todo/InProgress/Doneの3カラムを保持し、
/// ドラッグ&ドロップによるカラム間のタスク移動を処理する。
/// </summary>
public class MainViewModel : ObservableObject
{
	/// <summary>
	/// ViewModelを初期化し、3カラム(Todo/InProgress/Done)を用意する。
	/// </summary>
	public MainViewModel()
	{
		TodoColumn = new TaskColumnViewModel("未着手", KanbanStatus.Todo);
		InProgressColumn = new TaskColumnViewModel("対応中", KanbanStatus.InProgress);
		DoneColumn = new TaskColumnViewModel("完了", KanbanStatus.Done);
		MoveTaskCommand = new RelayCommand<MoveTaskRequest>(
			request =>
			{
				// CanExecuteはnullでfalseを返すが、Executeが直接呼ばれた場合にも
				// 備えて念のため防御的にガードする(公開コマンドとして誤用され得るため)。
				if (request is null)
				{
					return;
				}

				MoveTask(request.Task, request.TargetStatus);
			},
			request => request is not null);
	}

	/// <summary>未着手カラム。</summary>
	public TaskColumnViewModel TodoColumn { get; }

	/// <summary>対応中カラム。</summary>
	public TaskColumnViewModel InProgressColumn { get; }

	/// <summary>完了カラム。</summary>
	public TaskColumnViewModel DoneColumn { get; }

	/// <summary>
	/// ドラッグ&ドロップの結果を受け取り、カラム間のタスク移動を行うコマンド。
	/// <see cref="Behaviors.DragDropBehavior"/> のドロップ処理から実行される。
	/// </summary>
	public RelayCommand<MoveTaskRequest> MoveTaskCommand { get; }

	/// <summary>
	/// 指定したタスクを、現在所属しているカラムから <paramref name="targetStatus"/> のカラムへ移動する。
	/// 移動先が現在のカラムと同じ場合は何もしない。
	/// </summary>
	/// <param name="task">移動対象のタスク。</param>
	/// <param name="targetStatus">移動先の状態(カラム)。</param>
	public void MoveTask(TaskItem task, KanbanStatus targetStatus)
	{
		if (task.Status == targetStatus)
		{
			return;
		}

		var sourceColumn = GetColumn(task.Status);
		var targetColumn = GetColumn(targetStatus);

		sourceColumn.Tasks.Remove(task);
		task.Status = targetStatus;
		targetColumn.Tasks.Add(task);
	}

	private TaskColumnViewModel GetColumn(KanbanStatus status) => status switch
	{
		KanbanStatus.Todo => TodoColumn,
		KanbanStatus.InProgress => InProgressColumn,
		KanbanStatus.Done => DoneColumn,
		_ => throw new ArgumentOutOfRangeException(nameof(status), status, null),
	};
}
