using System.Globalization;
using System.Windows.Data;

namespace HabitTracker.Converters;

/// <summary>
/// 達成率(0.0〜1.0)を簡易棒グラフの棒の高さ(px)に変換するコンバーター。
/// 0%でも棒の存在が視認できるよう、最小高さを確保する。
/// </summary>
public class RateToHeightConverter : IValueConverter
{
	/// <summary>棒グラフの最大高さ(px)。達成率100%のときの高さ。</summary>
	public const double MaxHeight = 120;

	private const double MinHeight = 2;

	/// <inheritdoc/>
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		value is double rate ? Math.Max(MinHeight, rate * MaxHeight) : MinHeight;

	/// <inheritdoc/>
	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		throw new NotSupportedException();
}
