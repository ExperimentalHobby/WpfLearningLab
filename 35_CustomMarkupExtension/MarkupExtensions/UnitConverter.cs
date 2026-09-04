namespace CustomMarkupExtension.MarkupExtensions;

/// <summary>
/// 長さの単位変換を行う純粋なロジック。<see cref="UnitConversionExtension"/>から利用する。
/// WPFのデバイス非依存ピクセル(1px = 1/96インチ)を基準単位とする。
/// </summary>
public static class UnitConverter
{
	private const double PixelsPerInch = 96.0;
	private const double CentimetersPerInch = 2.54;

	/// <summary>
	/// <paramref name="value"/>(<paramref name="from"/>単位)を<paramref name="to"/>単位に変換する。
	/// </summary>
	public static double Convert(double value, UnitOfLength from, UnitOfLength to)
	{
		var pixels = ToPixels(value, from);
		return FromPixels(pixels, to);
	}

	private static double ToPixels(double value, UnitOfLength unit) => unit switch
	{
		UnitOfLength.Pixel => value,
		UnitOfLength.Inch => value * PixelsPerInch,
		UnitOfLength.Centimeter => value / CentimetersPerInch * PixelsPerInch,
		_ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null),
	};

	private static double FromPixels(double pixels, UnitOfLength unit) => unit switch
	{
		UnitOfLength.Pixel => pixels,
		UnitOfLength.Inch => pixels / PixelsPerInch,
		UnitOfLength.Centimeter => pixels / PixelsPerInch * CentimetersPerInch,
		_ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null),
	};
}
