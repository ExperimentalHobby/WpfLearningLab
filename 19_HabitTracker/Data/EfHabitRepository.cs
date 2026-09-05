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
		// (HabitId, Date)には一意インデックスを設定しており、通常はSetLog経由で重複行が
		// 作成されることはないが、DBの手動編集やスキーマ不整合等の外部要因で万一重複行が
		// 存在した場合にSingleOrDefaultだとInvalidOperationExceptionでクラッシュしてしまう。
		// FirstOrDefaultにすることで、その場合でもクラッシュせず先頭の1件を扱う。
		var log = _context.HabitLogs.FirstOrDefault(l => l.HabitId == habitId && l.Date == date);
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
