using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using LocalTaskScheduler.Models;
using LocalTaskScheduler.Services;

namespace LocalTaskScheduler.ViewModels;

/// <summary>
/// タスクの登録・一覧管理・実行(期日到来チェック)を行うメイン画面のViewModel。
/// </summary>
public class MainViewModel : ObservableObject
{
	private const string DateTimeFormat = "yyyy-MM-dd HH:mm";

	private readonly IToastNotifier _toastNotifier;
	private readonly ITaskRepository _taskRepository;

	private string _newTaskName = string.Empty;
	private ScheduleType _newTaskScheduleType = ScheduleType.Once;
	private string _newTaskExecuteAt = string.Empty;
	private string _newTaskIntervalMinutes = string.Empty;

	/// <summary>
	/// ViewModelを初期化する。保存済みのタスク一覧があれば読み込んで反映する。
	/// </summary>
	/// <param name="toastNotifier">トースト通知処理。</param>
	/// <param name="taskRepository">タスク一覧の永続化処理。</param>
	public MainViewModel(IToastNotifier toastNotifier, ITaskRepository taskRepository)
	{
		_toastNotifier = toastNotifier;
		_taskRepository = taskRepository;
		AddTaskCommand = new RelayCommand(AddTask, CanAddTask);
		RemoveTaskCommand = new RelayCommand<ScheduledTask>(RemoveTask);

		foreach (var task in _taskRepository.Load())
		{
			Tasks.Add(task);
		}
	}

	/// <summary>新規タスク入力フォームのタスク名。</summary>
	public string NewTaskName
	{
		get => _newTaskName;
		set
		{
			if (SetProperty(ref _newTaskName, value))
			{
				AddTaskCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>新規タスク入力フォームのスケジュール種別。</summary>
	public ScheduleType NewTaskScheduleType
	{
		get => _newTaskScheduleType;
		set
		{
			if (SetProperty(ref _newTaskScheduleType, value))
			{
				AddTaskCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>新規タスク入力フォームの実行日時("yyyy-MM-dd HH:mm"形式の文字列、Once種別用)。</summary>
	public string NewTaskExecuteAt
	{
		get => _newTaskExecuteAt;
		set
		{
			if (SetProperty(ref _newTaskExecuteAt, value))
			{
				AddTaskCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>新規タスク入力フォームの実行間隔(分、Interval種別用)。</summary>
	public string NewTaskIntervalMinutes
	{
		get => _newTaskIntervalMinutes;
		set
		{
			if (SetProperty(ref _newTaskIntervalMinutes, value))
			{
				AddTaskCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>登録済みのタスク一覧。</summary>
	public ObservableCollection<ScheduledTask> Tasks { get; } = [];

	/// <summary>タスク実行ログ(新しい順)。</summary>
	public ObservableCollection<string> ExecutionLog { get; } = [];

	/// <summary>入力欄の内容からタスクを追加するコマンド。</summary>
	public RelayCommand AddTaskCommand { get; }

	/// <summary>指定したタスクを削除するコマンド。</summary>
	public RelayCommand<ScheduledTask> RemoveTaskCommand { get; }

	/// <summary>
	/// 指定した時刻の時点で実行対象のタスクを全て実行する。
	/// View側のバックグラウンドタイマーから一定間隔で呼ばれることを想定する。
	/// </summary>
	/// <param name="now">現在時刻。</param>
	public void CheckDueTasks(DateTime now)
	{
		foreach (var task in Tasks.Where(t => TaskDueEvaluator.IsDue(t, now)).ToList())
		{
			ExecuteTask(task, now);
		}
	}

	private bool CanAddTask()
	{
		if (string.IsNullOrWhiteSpace(NewTaskName))
		{
			return false;
		}

		return NewTaskScheduleType switch
		{
			ScheduleType.Once => DateTime.TryParseExact(
				NewTaskExecuteAt, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out _),
			ScheduleType.Interval => double.TryParse(NewTaskIntervalMinutes, out var minutes) && minutes > 0,
			_ => false,
		};
	}

	private void AddTask()
	{
		ScheduledTask task = NewTaskScheduleType switch
		{
			ScheduleType.Once => new ScheduledTask(
				NewTaskName,
				ScheduleType.Once,
				DateTime.ParseExact(NewTaskExecuteAt, DateTimeFormat, CultureInfo.InvariantCulture),
				null),
			ScheduleType.Interval => new ScheduledTask(
				NewTaskName,
				ScheduleType.Interval,
				null,
				TimeSpan.FromMinutes(double.Parse(NewTaskIntervalMinutes, CultureInfo.InvariantCulture))),
			_ => throw new InvalidOperationException($"未対応のScheduleTypeです: {NewTaskScheduleType}"),
		};

		Tasks.Add(task);
		NewTaskName = string.Empty;
		NewTaskExecuteAt = string.Empty;
		NewTaskIntervalMinutes = string.Empty;
		SaveTasks();
	}

	private void RemoveTask(ScheduledTask? task)
	{
		if (task is not null)
		{
			Tasks.Remove(task);
			SaveTasks();
		}
	}

	private void ExecuteTask(ScheduledTask task, DateTime now)
	{
		task.LastExecutedAt = now;
		if (task.ScheduleType == ScheduleType.Once)
		{
			task.IsEnabled = false;
		}

		var message = $"タスク「{task.Name}」を実行しました。";
		try
		{
			_toastNotifier.Show("ローカルタスクスケジューラ", message);
		}
		catch (Exception)
		{
			// 通知APIが無効な環境(トースト通知が無効化されている等)でも、タスクの実行記録自体は
			// 失われないようにする。通知に失敗したこと自体はログに残さず実行ログのみ記録する。
		}

		ExecutionLog.Insert(0, $"{now:HH:mm:ss} {message}");
		SaveTasks();
	}

	private void SaveTasks() => _taskRepository.Save(Tasks.ToList());
}
