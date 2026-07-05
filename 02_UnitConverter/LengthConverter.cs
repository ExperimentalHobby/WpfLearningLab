namespace UnitConverter;

/// <summary>
/// m(メートル)・cm・inch・feet の相互変換を行うコンバータ。
/// 各単位の基準単位(メートル)への換算係数を使い、いったんメートルを経由して変換する。
/// </summary>
public class LengthConverter : IUnitConverter
{
	// 各単位 1 に相当するメートル数。
	private static readonly Dictionary<string, decimal> MetersPerUnit = new()
	{
		["m"] = 1m,
		["cm"] = 0.01m,
		["inch"] = 0.0254m,
		["feet"] = 0.3048m,
	};

	/// <inheritdoc />
	public IReadOnlyList<string> Units { get; } = new[] { "m", "cm", "inch", "feet" };

	/// <inheritdoc />
	public decimal Convert(decimal value, string fromUnit, string toUnit)
	{
		var meters = value * MetersPerUnit[fromUnit];
		return meters / MetersPerUnit[toUnit];
	}
}
