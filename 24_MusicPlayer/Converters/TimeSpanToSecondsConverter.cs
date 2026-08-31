using System.Globalization;
using System.Windows.Data;

namespace MusicPlayer.Converters;

/// <summary>
/// <see cref="TimeSpan"/>とスライダー等で使う秒数(<see cref="double"/>)を相互変換するコンバーター。
/// </summary>
public class TimeSpanToSecondsConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		value is TimeSpan timeSpan ? timeSpan.TotalSeconds : 0.0;

	/// <inheritdoc/>
	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		value is double seconds ? TimeSpan.FromSeconds(seconds) : TimeSpan.Zero;
}
