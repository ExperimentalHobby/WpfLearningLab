namespace UnitConverter;

/// <summary>
/// 摂氏・華氏・ケルビンの相互変換を行うコンバータ。
/// 一旦すべて摂氏に変換してから目的の単位に変換する(基準単位経由方式)。
/// </summary>
public class TemperatureConverter : IUnitConverter
{
	/// <inheritdoc />
	public IReadOnlyList<string> Units { get; } = new[] { "摂氏", "華氏", "ケルビン" };

	/// <inheritdoc />
	public decimal Convert(decimal value, string fromUnit, string toUnit)
	{
		var celsius = ToCelsius(value, fromUnit);
		return FromCelsius(celsius, toUnit);
	}

	private static decimal ToCelsius(decimal value, string unit) => unit switch
	{
		"摂氏" => value,
		"華氏" => (value - 32m) * 5m / 9m,
		"ケルビン" => value - 273.15m,
		_ => throw new ArgumentException($"未対応の単位です: {unit}", nameof(unit)),
	};

	private static decimal FromCelsius(decimal celsius, string unit) => unit switch
	{
		"摂氏" => celsius,
		"華氏" => celsius * 9m / 5m + 32m,
		"ケルビン" => celsius + 273.15m,
		_ => throw new ArgumentException($"未対応の単位です: {unit}", nameof(unit)),
	};
}
