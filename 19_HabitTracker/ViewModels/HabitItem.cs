namespace HabitTracker.ViewModels;

/// <summary>
/// 一覧表示用の習慣ラッパー。当日の実施状況と直近の達成率を保持する。
/// </summary>
public class HabitItem : ObservableObject
{
	private bool _isCompletedToday;
	private double _recentRate;

	/// <summary>
	/// 習慣を初期化する。
	/// </summary>
	/// <param name="id">習慣ID。</param>
	/// <param name="name">習慣名。</param>
	public HabitItem(int id, string name)
	{
		Id = id;
		Name = name;
	}

	/// <summary>習慣ID。</summary>
	public int Id { get; }

	/// <summary>習慣名。</summary>
	public string Name { get; }

	/// <summary>当日実施済みかどうか。</summary>
	public bool IsCompletedToday
	{
		get => _isCompletedToday;
		set => SetProperty(ref _isCompletedToday, value);
	}

	/// <summary>直近期間の達成率(0.0〜1.0)。</summary>
	public double RecentRate
	{
		get => _recentRate;
		set => SetProperty(ref _recentRate, value);
	}
}
