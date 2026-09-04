using System.Globalization;
using System.Windows.Data;

namespace ContactManager.Converters;

/// <summary>
/// 値が<see langword="null"/>かどうかを<see cref="bool"/>に変換する。
/// 「選択中の連絡先がある場合のみ編集フォームを有効化する」ために使う。
/// </summary>
public class NullToBooleanConverter : IValueConverter
{
	/// <summary>共有インスタンス(XAMLの<c>x:Static</c>から参照する)。</summary>
	public static readonly NullToBooleanConverter Instance = new();

	/// <inheritdoc/>
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is not null;

	/// <inheritdoc/>
	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		=> throw new NotSupportedException();
}
