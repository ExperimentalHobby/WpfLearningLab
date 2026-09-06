using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LocalTaskScheduler.Models;

/// <summary>
/// スケジュール登録されたタスク。実行状態(最終実行日時・有効/無効)の変更をUIへ通知する。
/// 永続化を持たないアプリのため、DBエンティティとUI表示用ラッパーを分けず1クラスで表現する。
/// </summary>
public class ScheduledTask : INotifyPropertyChanged
{
	private DateTime? _lastExecutedAt;
	private bool _isEnabled = true;

	/// <summary>
	/// タスクを初期化する。
	/// </summary>
	/// <param name="name">タスク名。</param>
	/// <param name="scheduleType">スケジュール種別。</param>
	/// <param name="executeAt"><see cref="ScheduleType.Once"/>の場合の実行日時。</param>
	/// <param name="interval"><see cref="ScheduleType.Interval"/>の場合の実行間隔。</param>
	public ScheduledTask(string name, ScheduleType scheduleType, DateTime? executeAt, TimeSpan? interval)
		: this(Guid.NewGuid(), name, scheduleType, executeAt, interval)
	{
	}

	/// <summary>
	/// タスクを初期化する(IDを明示指定する版)。永続化からの復元用。
	/// </summary>
	/// <param name="id">タスクを一意に識別するID。</param>
	/// <param name="name">タスク名。</param>
	/// <param name="scheduleType">スケジュール種別。</param>
	/// <param name="executeAt"><see cref="ScheduleType.Once"/>の場合の実行日時。</param>
	/// <param name="interval"><see cref="ScheduleType.Interval"/>の場合の実行間隔。</param>
	public ScheduledTask(Guid id, string name, ScheduleType scheduleType, DateTime? executeAt, TimeSpan? interval)
	{
		Id = id;
		Name = name;
		ScheduleType = scheduleType;
		ExecuteAt = executeAt;
		Interval = interval;
	}

	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;

	/// <summary>タスクを一意に識別するID。</summary>
	public Guid Id { get; }

	/// <summary>タスク名。</summary>
	public string Name { get; }

	/// <summary>スケジュール種別。</summary>
	public ScheduleType ScheduleType { get; }

	/// <summary><see cref="ScheduleType.Once"/>の場合の実行日時。</summary>
	public DateTime? ExecuteAt { get; }

	/// <summary><see cref="ScheduleType.Interval"/>の場合の実行間隔。</summary>
	public TimeSpan? Interval { get; }

	/// <summary>最終実行日時。未実行の場合は<see langword="null"/>。</summary>
	public DateTime? LastExecutedAt
	{
		get => _lastExecutedAt;
		set
		{
			if (_lastExecutedAt != value)
			{
				_lastExecutedAt = value;
				OnPropertyChanged();
			}
		}
	}

	/// <summary>有効かどうか。無効なタスクは実行対象にならない。</summary>
	public bool IsEnabled
	{
		get => _isEnabled;
		set
		{
			if (_isEnabled != value)
			{
				_isEnabled = value;
				OnPropertyChanged();
			}
		}
	}

	private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
