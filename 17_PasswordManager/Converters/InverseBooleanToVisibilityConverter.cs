using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PasswordManager.Converters;

/// <summary>
/// <see langword="bool"/> を反転してから<see cref="Visibility"/>に変換するコンバーター。
/// ロック画面(IsUnlocked=falseのときに表示)のような、真偽が逆転した表示切替に使う。
/// </summary>
public class InverseBooleanToVisibilityConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		value is bool boolValue && !boolValue ? Visibility.Visible : Visibility.Collapsed;

	/// <inheritdoc/>
	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		throw new NotSupportedException();
}
