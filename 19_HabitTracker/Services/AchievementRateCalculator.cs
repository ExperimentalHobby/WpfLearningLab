using HabitTracker.Models;

namespace HabitTracker.Services;

/// <summary>
/// 習慣の実施ログから達成率を集計する、DBに依存しない純粋なロジック。
/// </summary>
public static class AchievementRateCalculator
{
	/// <summary>
	/// 指定した期間における1つの習慣の達成率(実施日数 / 期間日数)を計算する。
	/// </summary>
	/// <param name="logs">対象習慣の実施ログ。</param>
	/// <param name="rangeStart">集計期間の開始日(含む)。</param>
	/// <param name="rangeEnd">集計期間の終了日(含む)。</param>
	/// <returns>0.0〜1.0の達成率。期間が不正な場合は0。</returns>
	public static double CalculateRate(IEnumerable<HabitLog> logs, DateOnly rangeStart, DateOnly rangeEnd)
	{
		var totalDays = rangeEnd.DayNumber - rangeStart.DayNumber + 1;
		if (totalDays <= 0)
		{
			return 0;
		}

		var completedDays = logs.Count(log => log.Date >= rangeStart && log.Date <= rangeEnd && log.IsCompleted);
		return (double)completedDays / totalDays;
	}

	/// <summary>
	/// 指定した期間の日別に、全習慣に対する達成率(実施した習慣数 / 全習慣数)を計算する。
	/// グラフ表示用の日次シリーズを作る。
	/// </summary>
	/// <param name="habits">ログを含む全習慣。</param>
	/// <param name="rangeStart">集計期間の開始日(含む)。</param>
	/// <param name="rangeEnd">集計期間の終了日(含む)。</param>
	public static IReadOnlyList<DailyRate> CalculateDailySeries(IReadOnlyList<Habit> habits, DateOnly rangeStart, DateOnly rangeEnd)
	{
		var series = new List<DailyRate>();
		for (var date = rangeStart; date <= rangeEnd; date = date.AddDays(1))
		{
			var rate = habits.Count == 0
				? 0
				: (double)habits.Count(habit => habit.Logs.Any(log => log.Date == date && log.IsCompleted)) / habits.Count;
			series.Add(new DailyRate(date, rate));
		}

		return series;
	}
}
