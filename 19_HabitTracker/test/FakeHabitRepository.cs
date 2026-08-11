using HabitTracker.Data;
using HabitTracker.Models;

namespace HabitTracker.Tests;

/// <summary>
/// <see cref="ViewModels.MainViewModel"/> のテスト用に使う<see cref="IHabitRepository"/>のフェイク実装。
/// メモリ上のリストでCRUDを模倣する。
/// </summary>
public class FakeHabitRepository : IHabitRepository
{
	private readonly List<Habit> _habits = [];
	private int _nextHabitId = 1;
	private int _nextLogId = 1;

	public IReadOnlyList<Habit> GetAllWithLogs() => _habits.Select(Clone).ToList();

	public void AddHabit(Habit habit)
	{
		habit.Id = _nextHabitId++;
		_habits.Add(habit);
	}

	public void DeleteHabit(int habitId) => _habits.RemoveAll(h => h.Id == habitId);

	public void SetLog(int habitId, DateOnly date, bool isCompleted)
	{
		var habit = _habits.FirstOrDefault(h => h.Id == habitId);
		if (habit is null)
		{
			return;
		}

		var log = habit.Logs.FirstOrDefault(l => l.Date == date);
		if (log is null)
		{
			habit.Logs.Add(new HabitLog { Id = _nextLogId++, HabitId = habitId, Date = date, IsCompleted = isCompleted });
		}
		else
		{
			log.IsCompleted = isCompleted;
		}
	}

	// 実リポジトリ(AsNoTracking)と同様に、GetAllWithLogsの戻り値を経由した呼び出し元からの
	// 内部状態への意図しない書き換えを防ぐため複製を返す。
	private static Habit Clone(Habit habit) => new()
	{
		Id = habit.Id,
		Name = habit.Name,
		Logs = habit.Logs.Select(l => new HabitLog { Id = l.Id, HabitId = l.HabitId, Date = l.Date, IsCompleted = l.IsCompleted }).ToList(),
	};
}
