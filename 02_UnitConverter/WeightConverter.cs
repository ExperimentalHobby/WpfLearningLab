namespace UnitConverter;

/// <summary>
/// kg(キログラム)・g・lb の相互変換を行うコンバータ。
/// 各単位の基準単位(キログラム)への換算係数を使い、いったんキログラムを経由して変換する。
/// </summary>
public class WeightConverter : IUnitConverter
{
	// 各単位 1 に相当するキログラム数。
	private static readonly Dictionary<string, decimal> KilogramsPerUnit = new()
	{
		["kg"] = 1m,
		["g"] = 0.001m,
		["lb"] = 0.45359237m,
	};

	/// <inheritdoc />
	public IReadOnlyList<string> Units { get; } = new[] { "kg", "g", "lb" };

	/// <inheritdoc />
	public decimal Convert(decimal value, string fromUnit, string toUnit)
	{
		var kilograms = value * KilogramsPerUnit[fromUnit];
		return kilograms / KilogramsPerUnit[toUnit];
	}
}
