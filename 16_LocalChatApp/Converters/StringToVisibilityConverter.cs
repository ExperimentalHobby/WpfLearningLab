using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LocalChatApp.Converters;

/// <summary>
/// 文字列が空でなければ<see cref="Visibility.Visible"/>、空(または<see langword="null"/>)なら
/// <see cref="Visibility.Collapsed"/>に変換する。エラーメッセージ欄の出し分けに使う。
/// </summary>
public class StringToVisibilityConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;

	/// <inheritdoc/>
	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		throw new NotSupportedException();
}
