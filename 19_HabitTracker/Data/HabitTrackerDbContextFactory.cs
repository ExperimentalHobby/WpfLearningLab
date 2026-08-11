using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HabitTracker.Data;

/// <summary>
/// <c>dotnet ef migrations</c> 実行時(デザインタイム)専用に<see cref="HabitTrackerDbContext"/>を生成するファクトリ。
/// 本アプリはASP.NET Coreのようなホスト/DIコンテナを持たないため、EF Coreツールがコンテキストを
/// 生成できるようこのファクトリを明示的に用意する。アプリ実行時には使われない。
/// </summary>
public class HabitTrackerDbContextFactory : IDesignTimeDbContextFactory<HabitTrackerDbContext>
{
	/// <inheritdoc/>
	public HabitTrackerDbContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<HabitTrackerDbContext>();
		optionsBuilder.UseSqlite("Data Source=design-time.db");
		return new HabitTrackerDbContext(optionsBuilder.Options);
	}
}
