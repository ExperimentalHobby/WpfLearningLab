namespace HabitTracker.Services;

/// <summary>
/// ある1日における全習慣の達成率。グラフ表示用のシリーズの1点を表す。
/// </summary>
/// <param name="Date">対象日。</param>
/// <param name="Rate">達成率(0.0〜1.0)。</param>
public record DailyRate(DateOnly Date, double Rate);
