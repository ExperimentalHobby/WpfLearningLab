using HabitTracker.Models;
using HabitTracker.ViewModels;

namespace HabitTracker.Tests;

/// <summary>
/// <see cref="MainViewModel"/> の単体テスト。フェイクの<see cref="FakeHabitRepository"/>で検証する。
/// 「今日」は<c>2026-08-15</c>固定でViewModelに注入し、達成率計算を決定的にする。
/// </summary>
public class MainViewModelTests
{
	private static readonly DateOnly Today = new(2026, 8, 15);

	private static MainViewModel CreateViewModel(FakeHabitRepository? repository = null) =>
		new(repository ?? new FakeHabitRepository(), () => Today);

	/// <summary>
	/// パス条件: AddHabitCommand実行で入力した習慣名が一覧に追加されること
	/// </summary>
	[Fact]
	public void AddHabitCommand_入力した習慣名が一覧に追加される()
	{
		var viewModel = CreateViewModel();
		viewModel.InputHabitName = "運動";

		viewModel.AddHabitCommand.Execute(null);

		Assert.Single(viewModel.Habits);
		Assert.Equal("運動", viewModel.Habits[0].Name);
	}

	/// <summary>
	/// パス条件: 習慣名が空欄の場合、AddHabitCommandのCanExecuteがfalseになること
	/// </summary>
	[Fact]
	public void AddHabitCommand_習慣名が空欄だとCanExecuteがfalseになる()
	{
		var viewModel = CreateViewModel();
		viewModel.InputHabitName = "   ";

		Assert.False(viewModel.AddHabitCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: DeleteHabitCommand実行で指定した習慣が一覧から削除されること
	/// </summary>
	[Fact]
	public void DeleteHabitCommand_指定した習慣が一覧から削除される()
	{
		var viewModel = CreateViewModel();
		viewModel.InputHabitName = "運動";
		viewModel.AddHabitCommand.Execute(null);
		var item = viewModel.Habits[0];

		viewModel.DeleteHabitCommand.Execute(item);

		Assert.Empty(viewModel.Habits);
	}

	/// <summary>
	/// パス条件: ToggleTodayCommand実行で当日の実施状況が切り替わりリポジトリに反映されること
	/// </summary>
	[Fact]
	public void ToggleTodayCommand_当日の実施状況が切り替わる()
	{
		var repository = new FakeHabitRepository();
		var viewModel = CreateViewModel(repository);
		viewModel.InputHabitName = "運動";
		viewModel.AddHabitCommand.Execute(null);
		var item = viewModel.Habits[0];

		viewModel.ToggleTodayCommand.Execute(item);

		Assert.True(viewModel.Habits[0].IsCompletedToday);
		var stored = repository.GetAllWithLogs()[0];
		Assert.Single(stored.Logs);
		Assert.True(stored.Logs.First().IsCompleted);
	}

	/// <summary>
	/// パス条件: ToggleTodayCommandをもう一度実行すると実施状況が元に戻ること
	/// </summary>
	[Fact]
	public void ToggleTodayCommand_再度実行すると実施状況が元に戻る()
	{
		var viewModel = CreateViewModel();
		viewModel.InputHabitName = "運動";
		viewModel.AddHabitCommand.Execute(null);
		var item = viewModel.Habits[0];
		viewModel.ToggleTodayCommand.Execute(item);

		viewModel.ToggleTodayCommand.Execute(viewModel.Habits[0]);

		Assert.False(viewModel.Habits[0].IsCompletedToday);
	}

	/// <summary>
	/// パス条件: ToggleTodayCommand実行後、該当習慣の直近達成率が再計算されること
	/// </summary>
	[Fact]
	public void ToggleTodayCommand_実行後に達成率が再計算される()
	{
		var viewModel = CreateViewModel();
		viewModel.InputHabitName = "運動";
		viewModel.AddHabitCommand.Execute(null);
		var item = viewModel.Habits[0];
		Assert.Equal(0.0, item.RecentRate);

		viewModel.ToggleTodayCommand.Execute(item);

		Assert.Equal(1.0 / MainViewModel.RecentDayCount, viewModel.Habits[0].RecentRate);
	}

	/// <summary>
	/// パス条件: 初期化時にリポジトリの内容が一覧・グラフ用シリーズに読み込まれること
	/// </summary>
	[Fact]
	public void コンストラクタ_初期化時にリポジトリの内容が読み込まれる()
	{
		var repository = new FakeHabitRepository();
		var habit = new Habit { Name = "運動" };
		repository.AddHabit(habit);
		repository.SetLog(habit.Id, Today, true);

		var viewModel = CreateViewModel(repository);

		Assert.Single(viewModel.Habits);
		Assert.True(viewModel.Habits[0].IsCompletedToday);
		Assert.Equal(MainViewModel.RecentDayCount, viewModel.DailySeries.Count);
		Assert.Equal(1.0, viewModel.DailySeries[^1].Rate);
	}

	/// <summary>
	/// パス条件: 習慣が1件も登録されていない場合、一覧・グラフ用シリーズが空/0で初期化されること
	/// </summary>
	[Fact]
	public void コンストラクタ_習慣が未登録の場合一覧は空でグラフは全て0になる()
	{
		var viewModel = CreateViewModel();

		Assert.Empty(viewModel.Habits);
		Assert.Equal(MainViewModel.RecentDayCount, viewModel.DailySeries.Count);
		Assert.All(viewModel.DailySeries, point => Assert.Equal(0.0, point.Rate));
	}
}
