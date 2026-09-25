using System.Globalization;
using System.Windows.Data;

namespace TaskApiWorkbench.Client.Converters;

/// <summary>
/// <see cref="Models.TaskItem.IsCompleted"/> を「完了」/「未完了」の表示文字列に変換する。
/// </summary>
public sealed class BoolToStatusConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		value is true ? "完了" : "未完了";

	/// <inheritdoc/>
	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		throw new NotSupportedException();
}
