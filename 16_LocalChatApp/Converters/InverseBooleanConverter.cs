using System.Globalization;
using System.Windows.Data;

namespace LocalChatApp.Converters;

/// <summary>
/// bool値を反転する。接続済みのときに接続設定欄を無効化する、といった用途に使う。
/// </summary>
public class InverseBooleanConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		value is bool b && !b;

	/// <inheritdoc/>
	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		value is bool b && !b;
}
