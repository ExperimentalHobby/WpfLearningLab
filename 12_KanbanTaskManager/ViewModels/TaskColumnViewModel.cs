using System.Collections.ObjectModel;
using KanbanTaskManager.Models;

namespace KanbanTaskManager.ViewModels;

/// <summary>
/// カンバンボード1カラム分の状態(タスク一覧・新規タスク入力・追加/削除コマンド)を保持するViewModel。
/// </summary>
public class TaskColumnViewModel : ObservableObject
{
	private string _newTaskTitle = string.Empty;

	/// <summary>
	/// カラムを初期化する。
	/// </summary>
	/// <param name="title">カラムの表示名(例: 未着手)。</param>
	/// <param name="status">このカラムが表すタスク状態。</param>
	public TaskColumnViewModel(string title, KanbanStatus status)
	{
		Title = title;
		Status = status;
		AddCommand = new RelayCommand(Add, CanAdd);
		DeleteCommand = new RelayCommand<TaskItem>(Delete);
	}

	/// <summary>カラムの表示名。</summary>
	public string Title { get; }

	/// <summary>このカラムが表すタスク状態。</summary>
	public KanbanStatus Status { get; }

	/// <summary>このカラムに属するタスクの一覧。</summary>
	public ObservableCollection<TaskItem> Tasks { get; } = [];

	/// <summary>新規タスク追加用の入力欄。</summary>
	public string NewTaskTitle
	{
		get => _newTaskTitle;
		set
		{
			if (SetProperty(ref _newTaskTitle, value))
			{
				AddCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>
	/// <see cref="NewTaskTitle"/> の内容でタスクを追加するコマンド。
	/// </summary>
	public RelayCommand AddCommand { get; }

	/// <summary>
	/// 指定したタスクをこのカラムから削除するコマンド。
	/// </summary>
	public RelayCommand<TaskItem> DeleteCommand { get; }

	private bool CanAdd() => !string.IsNullOrWhiteSpace(NewTaskTitle);

	private void Add()
	{
		Tasks.Add(new TaskItem { Title = NewTaskTitle, Status = Status });
		NewTaskTitle = string.Empty;
	}

	private void Delete(TaskItem? task)
	{
		if (task is not null)
		{
			Tasks.Remove(task);
		}
	}
}
