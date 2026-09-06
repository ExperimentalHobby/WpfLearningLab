using LocalTaskScheduler.Models;

namespace LocalTaskScheduler.Services;

/// <summary>
/// タスク一覧の永続化を担う処理の抽象。
/// </summary>
public interface ITaskRepository
{
	/// <summary>
	/// 保存済みのタスク一覧を読み込む。保存されたことがない場合は空の一覧を返す。
	/// </summary>
	IReadOnlyList<ScheduledTask> Load();

	/// <summary>
	/// タスク一覧を保存する(全件置き換え)。
	/// </summary>
	void Save(IReadOnlyList<ScheduledTask> tasks);
}
