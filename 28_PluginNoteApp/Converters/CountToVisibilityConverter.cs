using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PluginNoteApp.Converters;

/// <summary>
/// 件数(<see cref="int"/>)が1件以上であれば<see cref="Visibility.Visible"/>、
/// 0件であれば<see cref="Visibility.Collapsed"/>に変換するコンバーター。
/// </summary>
public class CountToVisibilityConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		value is int count && count > 0 ? Visibility.Visible : Visibility.Collapsed;

	/// <inheritdoc/>
	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		throw new NotSupportedException();
}
