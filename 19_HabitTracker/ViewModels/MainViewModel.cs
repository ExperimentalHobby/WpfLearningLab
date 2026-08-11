using System.Collections.ObjectModel;
using HabitTracker.Data;
using HabitTracker.Models;
using HabitTracker.Services;

namespace HabitTracker.ViewModels;

/// <summary>
/// 習慣トラッカーアプリのメイン画面のViewModel。習慣の登録・削除・日次チェックと達成率集計を担う。
/// </summary>
public class MainViewModel : ObservableObject
{
	/// <summary>達成率・グラフの集計対象とする直近日数。</summary>
	public const int RecentDayCount = 14;

	private readonly IHabitRepository _repository;
	private readonly Func<DateOnly> _getToday;

	private string _inputHabitName = string.Empty;

	/// <summary>
	/// ViewModelを初期化し、リポジトリから習慣一覧を読み込む。
	/// </summary>
	/// <param name="repository">習慣・実施ログの永続化を担うリポジトリ。</param>
	/// <param name="getToday">「今日」の日付を取得するデリゲート(省略時は実際の today)。テストで固定日付を注入する。</param>
	public MainViewModel(IHabitRepository repository, Func<DateOnly>? getToday = null)
	{
		_repository = repository;
		_getToday = getToday ?? (() => DateOnly.FromDateTime(DateTime.Today));

		AddHabitCommand = new RelayCommand(AddHabit, CanAddHabit);
		DeleteHabitCommand = new RelayCommand<HabitItem>(DeleteHabit);
		ToggleTodayCommand = new RelayCommand<HabitItem>(ToggleToday);

		LoadAll();
	}

	/// <summary>登録済み習慣の一覧。</summary>
	public ObservableCollection<HabitItem> Habits { get; } = [];

	/// <summary>直近<see cref="RecentDayCount"/>日間の日別達成率シリーズ(グラフ表示用)。</summary>
	public ObservableCollection<DailyRate> DailySeries { get; } = [];

	/// <summary>入力フォーム: 新規習慣名。</summary>
	public string InputHabitName
	{
		get => _inputHabitName;
		set
		{
			if (SetProperty(ref _inputHabitName, value))
			{
				AddHabitCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>入力フォームの習慣名を新規登録するコマンド。</summary>
	public RelayCommand AddHabitCommand { get; }

	/// <summary>指定した習慣を削除するコマンド。</summary>
	public RelayCommand<HabitItem> DeleteHabitCommand { get; }

	/// <summary>指定した習慣の当日の実施状況を切り替えるコマンド。</summary>
	public RelayCommand<HabitItem> ToggleTodayCommand { get; }

	private bool CanAddHabit() => !string.IsNullOrWhiteSpace(InputHabitName);

	private void AddHabit()
	{
		_repository.AddHabit(new Habit { Name = InputHabitName });
		InputHabitName = string.Empty;
		LoadAll();
	}

	private void DeleteHabit(HabitItem? item)
	{
		if (item is null)
		{
			return;
		}

		_repository.DeleteHabit(item.Id);
		LoadAll();
	}

	private void ToggleToday(HabitItem? item)
	{
		if (item is null)
		{
			return;
		}

		_repository.SetLog(item.Id, _getToday(), !item.IsCompletedToday);
		LoadAll();
	}

	private void LoadAll()
	{
		var today = _getToday();
		var rangeStart = today.AddDays(-(RecentDayCount - 1));
		var habits = _repository.GetAllWithLogs();

		Habits.Clear();
		foreach (var habit in habits)
		{
			Habits.Add(new HabitItem(habit.Id, habit.Name)
			{
				IsCompletedToday = habit.Logs.Any(log => log.Date == today && log.IsCompleted),
				RecentRate = AchievementRateCalculator.CalculateRate(habit.Logs, rangeStart, today),
			});
		}

		DailySeries.Clear();
		foreach (var point in AchievementRateCalculator.CalculateDailySeries(habits, rangeStart, today))
		{
			DailySeries.Add(point);
		}
	}
}
