using System.Globalization;
using System.Windows.Data;
using DragDropFileTagger.Services;

namespace DragDropFileTagger.Converters;

/// <summary>
/// バイト数(<see cref="long"/>)を<see cref="FileSizeFormatter"/>で整形した文字列に変換する。
/// </summary>
public class FileSizeConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		=> value is long bytes ? FileSizeFormatter.Format(bytes) : string.Empty;

	/// <inheritdoc/>
	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		=> throw new NotSupportedException();
}
