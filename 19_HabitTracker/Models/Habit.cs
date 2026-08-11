namespace HabitTracker.Models;

/// <summary>
/// 記録対象の習慣を表すエンティティ。
/// </summary>
public class Habit
{
	/// <summary>習慣ID(SQLiteの自動採番)。新規作成時は0。</summary>
	public int Id { get; set; }

	/// <summary>習慣名。</summary>
	public required string Name { get; set; }

	/// <summary>この習慣の日次実施ログ。</summary>
	public ICollection<HabitLog> Logs { get; set; } = [];
}
