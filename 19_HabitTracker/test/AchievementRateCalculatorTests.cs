using HabitTracker.Models;
using HabitTracker.Services;

namespace HabitTracker.Tests;

/// <summary>
/// <see cref="AchievementRateCalculator"/> の単体テスト。DBに依存しない純粋ロジックを検証する。
/// </summary>
public class AchievementRateCalculatorTests
{
	/// <summary>
	/// パス条件: 期間内の全日が実施済みの場合、達成率が1.0(100%)になること
	/// </summary>
	[Fact]
	public void CalculateRate_全日達成の場合達成率が1になる()
	{
		var start = new DateOnly(2026, 8, 1);
		var end = new DateOnly(2026, 8, 3);
		var logs = new List<HabitLog>
		{
			new() { Date = new DateOnly(2026, 8, 1), IsCompleted = true },
			new() { Date = new DateOnly(2026, 8, 2), IsCompleted = true },
			new() { Date = new DateOnly(2026, 8, 3), IsCompleted = true },
		};

		var rate = AchievementRateCalculator.CalculateRate(logs, start, end);

		Assert.Equal(1.0, rate);
	}

	/// <summary>
	/// パス条件: 一部の日のみ実施済みの場合、正しい達成率(小数)になること
	/// </summary>
	[Fact]
	public void CalculateRate_一部未達成の場合正しい達成率になる()
	{
		var start = new DateOnly(2026, 8, 1);
		var end = new DateOnly(2026, 8, 4);
		var logs = new List<HabitLog>
		{
			new() { Date = new DateOnly(2026, 8, 1), IsCompleted = true },
			new() { Date = new DateOnly(2026, 8, 2), IsCompleted = false },
			new() { Date = new DateOnly(2026, 8, 3), IsCompleted = true },
		};

		var rate = AchievementRateCalculator.CalculateRate(logs, start, end);

		Assert.Equal(0.5, rate);
	}

	/// <summary>
	/// パス条件: ログが1件も無い日は未達成として扱われること(達成率0)
	/// </summary>
	[Fact]
	public void CalculateRate_ログが無い場合達成率が0になる()
	{
		var start = new DateOnly(2026, 8, 1);
		var end = new DateOnly(2026, 8, 3);

		var rate = AchievementRateCalculator.CalculateRate([], start, end);

		Assert.Equal(0.0, rate);
	}

	/// <summary>
	/// パス条件: 習慣が0件の場合、日別シリーズの達成率が全て0になること(ゼロ除算しない)
	/// </summary>
	[Fact]
	public void CalculateDailySeries_習慣が0件の場合達成率が全て0になる()
	{
		var start = new DateOnly(2026, 8, 1);
		var end = new DateOnly(2026, 8, 2);

		var series = AchievementRateCalculator.CalculateDailySeries([], start, end);

		Assert.Equal(2, series.Count);
		Assert.All(series, item => Assert.Equal(0.0, item.Rate));
	}

	/// <summary>
	/// パス条件: 複数習慣の日別達成率シリーズが正しく計算されること
	/// </summary>
	[Fact]
	public void CalculateDailySeries_複数習慣の日別達成率が正しく計算される()
	{
		var day1 = new DateOnly(2026, 8, 1);
		var day2 = new DateOnly(2026, 8, 2);
		var habits = new List<Habit>
		{
			new()
			{
				Name = "運動",
				Logs = [new HabitLog { Date = day1, IsCompleted = true }, new HabitLog { Date = day2, IsCompleted = true }],
			},
			new()
			{
				Name = "読書",
				Logs = [new HabitLog { Date = day1, IsCompleted = false }, new HabitLog { Date = day2, IsCompleted = true }],
			},
		};

		var series = AchievementRateCalculator.CalculateDailySeries(habits, day1, day2);

		Assert.Equal(0.5, series[0].Rate);
		Assert.Equal(1.0, series[1].Rate);
	}
}
