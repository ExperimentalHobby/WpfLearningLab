using HabitTracker.Models;

namespace HabitTracker.Data;

/// <summary>
/// 習慣・実施ログの永続化を担うリポジトリの抽象。
/// </summary>
public interface IHabitRepository
{
	/// <summary>
	/// 登録済みの全習慣を、実施ログを含めて取得する。
	/// </summary>
	IReadOnlyList<Habit> GetAllWithLogs();

	/// <summary>
	/// 習慣を新規追加する。追加後、<paramref name="habit"/> の <see cref="Habit.Id"/> が更新される。
	/// </summary>
	void AddHabit(Habit habit);

	/// <summary>
	/// 指定したIDの習慣を削除する(関連する実施ログも削除される)。
	/// </summary>
	void DeleteHabit(int habitId);

	/// <summary>
	/// 指定した習慣・日付の実施状況を設定する。同じ習慣・日付のログが既に存在する場合は上書き更新し、
	/// 存在しない場合は新規作成する。
	/// </summary>
	void SetLog(int habitId, DateOnly date, bool isCompleted);
}
