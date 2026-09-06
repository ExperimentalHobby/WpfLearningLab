using System.Globalization;
using System.Windows.Data;

namespace ChartVisualization.Converters;

/// <summary>
/// 列挙値と<see cref="System.Windows.Controls.RadioButton"/>の<c>IsChecked</c>を相互変換するコンバーター。
/// <c>ConverterParameter</c>に比較対象の列挙値名(文字列)を指定して使う。
/// </summary>
public class EnumToBooleanConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is null || parameter is null)
		{
			return false;
		}

		return value.ToString() == parameter.ToString();
	}

	/// <inheritdoc/>
	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is true && parameter is not null && Enum.TryParse(targetType, parameter.ToString(), out var result))
		{
			return result;
		}

		return Binding.DoNothing;
	}
}
