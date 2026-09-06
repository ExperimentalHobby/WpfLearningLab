using LocalTaskScheduler.Models;

namespace LocalTaskScheduler.Services;

/// <summary>
/// <see cref="ScheduledTask"/>のJSON永続化用データ転送オブジェクト。
/// <see cref="ScheduledTask"/>自体はId等が読み取り専用でSystem.Text.Jsonの既定の
/// デシリアライズ(パラメーターなしコンストラクタ+可変プロパティ)に適さないため分離する。
/// </summary>
internal sealed class ScheduledTaskDto
{
	/// <summary>タスクを一意に識別するID。</summary>
	public Guid Id { get; set; }

	/// <summary>タスク名。</summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>スケジュール種別。</summary>
	public ScheduleType ScheduleType { get; set; }

	/// <summary><see cref="ScheduleType.Once"/>の場合の実行日時。</summary>
	public DateTime? ExecuteAt { get; set; }

	/// <summary><see cref="ScheduleType.Interval"/>の場合の実行間隔。</summary>
	public TimeSpan? Interval { get; set; }

	/// <summary>最終実行日時。</summary>
	public DateTime? LastExecutedAt { get; set; }

	/// <summary>有効かどうか。</summary>
	public bool IsEnabled { get; set; } = true;
}
