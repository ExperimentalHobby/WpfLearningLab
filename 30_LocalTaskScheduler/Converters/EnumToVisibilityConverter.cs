using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LocalTaskScheduler.Converters;

/// <summary>
/// 列挙値が<c>ConverterParameter</c>に指定した値(文字列)と一致する場合<see cref="Visibility.Visible"/>、
/// 一致しない場合<see cref="Visibility.Collapsed"/>に変換するコンバーター。
/// </summary>
public class EnumToVisibilityConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		value?.ToString() == parameter?.ToString() ? Visibility.Visible : Visibility.Collapsed;

	/// <inheritdoc/>
	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		throw new NotSupportedException();
}
