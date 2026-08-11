using HabitTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace HabitTracker.Data;

/// <summary>
/// EF Core(<see cref="HabitTrackerDbContext"/>)を使って習慣・実施ログをCRUDするリポジトリ。
/// </summary>
public class EfHabitRepository : IHabitRepository
{
	private readonly HabitTrackerDbContext _context;

	/// <summary>
	/// リポジトリを初期化する。
	/// </summary>
	/// <param name="context">習慣トラッカーのDbContext。</param>
	public EfHabitRepository(HabitTrackerDbContext context)
	{
		_context = context;
	}

	/// <inheritdoc/>
	public IReadOnlyList<Habit> GetAllWithLogs() =>
		_context.Habits.Include(habit => habit.Logs).AsNoTracking().OrderBy(habit => habit.Id).ToList();

	/// <inheritdoc/>
	public void AddHabit(Habit habit)
	{
		_context.Habits.Add(habit);
		_context.SaveChanges();
	}

	/// <inheritdoc/>
	public void DeleteHabit(int habitId)
	{
		var habit = _context.Habits.Find(habitId);
		if (habit is null)
		{
			return;
		}

		_context.Habits.Remove(habit);
		_context.SaveChanges();
	}

	/// <inheritdoc/>
	public void SetLog(int habitId, DateOnly date, bool isCompleted)
	{
		var log = _context.HabitLogs.SingleOrDefault(l => l.HabitId == habitId && l.Date == date);
		if (log is null)
		{
			_context.HabitLogs.Add(new HabitLog { HabitId = habitId, Date = date, IsCompleted = isCompleted });
		}
		else
		{
			log.IsCompleted = isCompleted;
		}

		_context.SaveChanges();
	}
}
