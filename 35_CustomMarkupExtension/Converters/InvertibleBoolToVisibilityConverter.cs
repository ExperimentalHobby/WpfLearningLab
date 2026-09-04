using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CustomMarkupExtension.Converters;

/// <summary>
/// <see langword="bool"/>を<see cref="Visibility"/>に変換する。
/// 標準の<see cref="System.Windows.Controls.BooleanToVisibilityConverter"/>と異なり、
/// <paramref name="parameter"/>に<see langword="true"/>を渡すことで結果を反転できる。
/// </summary>
public class InvertibleBoolToVisibilityConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		var boolValue = value is true;
		var invert = parameter is true;
		if (invert)
		{
			boolValue = !boolValue;
		}
		return boolValue ? Visibility.Visible : Visibility.Collapsed;
	}

	/// <inheritdoc/>
	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		=> throw new NotSupportedException();
}
