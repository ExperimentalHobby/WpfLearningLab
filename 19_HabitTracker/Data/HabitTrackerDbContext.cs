using HabitTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace HabitTracker.Data;

/// <summary>
/// 習慣トラッカーのSQLiteデータベースに対する<see cref="DbContext"/>。
/// </summary>
public class HabitTrackerDbContext : DbContext
{
	/// <summary>
	/// コンテキストを初期化する。
	/// </summary>
	/// <param name="options">接続先等のオプション。</param>
	public HabitTrackerDbContext(DbContextOptions<HabitTrackerDbContext> options)
		: base(options)
	{
	}

	/// <summary>登録済みの習慣。</summary>
	public DbSet<Habit> Habits => Set<Habit>();

	/// <summary>日次の実施ログ。</summary>
	public DbSet<HabitLog> HabitLogs => Set<HabitLog>();

	/// <inheritdoc/>
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<HabitLog>()
			.HasIndex(log => new { log.HabitId, log.Date })
			.IsUnique();

		modelBuilder.Entity<HabitLog>()
			.HasOne(log => log.Habit)
			.WithMany(habit => habit.Logs)
			.HasForeignKey(log => log.HabitId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
