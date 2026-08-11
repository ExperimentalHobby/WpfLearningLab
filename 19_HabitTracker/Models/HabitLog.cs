namespace HabitTracker.Models;

/// <summary>
/// 1日分の習慣実施記録を表すエンティティ。1つの<see cref="Habit"/>・日付の組につき1件のみ存在する。
/// </summary>
public class HabitLog
{
	/// <summary>ログID(SQLiteの自動採番)。新規作成時は0。</summary>
	public int Id { get; set; }

	/// <summary>対象の習慣ID。</summary>
	public int HabitId { get; set; }

	/// <summary>対象の習慣。</summary>
	public Habit Habit { get; set; } = null!;

	/// <summary>記録対象日。</summary>
	public required DateOnly Date { get; set; }

	/// <summary>その日に実施したかどうか。</summary>
	public bool IsCompleted { get; set; }
}
