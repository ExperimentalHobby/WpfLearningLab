using HabitTracker.Data;
using HabitTracker.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace HabitTracker.Tests;

/// <summary>
/// <see cref="EfHabitRepository"/> の単体テスト。
/// テストごとに一時ファイルへ実際にSQLite+EF Coreのマイグレーションでアクセスし、CRUDを検証する。
/// </summary>
public class EfHabitRepositoryTests : IDisposable
{
	private readonly string _dbPath;

	public EfHabitRepositoryTests()
	{
		_dbPath = Path.Combine(Path.GetTempPath(), $"HabitTrackerTests_{Guid.NewGuid():N}.db");
	}

	public void Dispose()
	{
		SqliteConnection.ClearAllPools();
		if (File.Exists(_dbPath))
		{
			File.Delete(_dbPath);
		}
	}

	private EfHabitRepository CreateRepository()
	{
		var optionsBuilder = new DbContextOptionsBuilder<HabitTrackerDbContext>();
		optionsBuilder.UseSqlite($"Data Source={_dbPath}");
		var context = new HabitTrackerDbContext(optionsBuilder.Options);
		context.Database.Migrate();
		return new EfHabitRepository(context);
	}

	/// <summary>
	/// パス条件: AddHabitで登録した習慣がGetAllWithLogsで取得できること
	/// </summary>
	[Fact]
	public void AddHabit_登録した習慣がGetAllWithLogsで取得できる()
	{
		var repository = CreateRepository();
		var habit = new Habit { Name = "運動" };

		repository.AddHabit(habit);
		var all = repository.GetAllWithLogs();

		Assert.Single(all);
		Assert.Equal("運動", all[0].Name);
		Assert.NotEqual(0, habit.Id);
	}

	/// <summary>
	/// パス条件: DeleteHabitで削除した習慣がGetAllWithLogsから消えること
	/// </summary>
	[Fact]
	public void DeleteHabit_削除した習慣がGetAllWithLogsから消える()
	{
		var repository = CreateRepository();
		var habit = new Habit { Name = "運動" };
		repository.AddHabit(habit);

		repository.DeleteHabit(habit.Id);
		var all = repository.GetAllWithLogs();

		Assert.Empty(all);
	}

	/// <summary>
	/// パス条件: SetLogで指定した習慣・日付のログが新規作成されること
	/// </summary>
	[Fact]
	public void SetLog_新規のログが作成される()
	{
		var repository = CreateRepository();
		var habit = new Habit { Name = "運動" };
		repository.AddHabit(habit);
		var date = new DateOnly(2026, 8, 1);

		repository.SetLog(habit.Id, date, true);
		var all = repository.GetAllWithLogs();

		Assert.Single(all[0].Logs);
		Assert.Equal(date, all[0].Logs.First().Date);
		Assert.True(all[0].Logs.First().IsCompleted);
	}

	/// <summary>
	/// パス条件: 同じ習慣・日付でSetLogを再度呼ぶと、新規作成されず既存ログが上書き更新されること
	/// </summary>
	[Fact]
	public void SetLog_同じ日付で再度呼ぶと上書き更新される()
	{
		var repository = CreateRepository();
		var habit = new Habit { Name = "運動" };
		repository.AddHabit(habit);
		var date = new DateOnly(2026, 8, 1);
		repository.SetLog(habit.Id, date, true);

		repository.SetLog(habit.Id, date, false);
		var all = repository.GetAllWithLogs();

		Assert.Single(all[0].Logs);
		Assert.False(all[0].Logs.First().IsCompleted);
	}
}
