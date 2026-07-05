namespace UnitConverter;

/// <summary>
/// カテゴリ名(温度・長さ・重さ)から対応する <see cref="IUnitConverter"/> を選び、
/// 単位一覧の取得と変換をまとめて行うファサード。UIはこのクラスだけを呼び出せばよい。
/// </summary>
public class UnitConverterEngine
{
	private readonly Dictionary<string, IUnitConverter> _converters = new()
	{
		["温度"] = new TemperatureConverter(),
		["長さ"] = new LengthConverter(),
		["重さ"] = new WeightConverter(),
	};

	/// <summary>
	/// 選択可能なカテゴリの一覧(温度・長さ・重さ)。
	/// </summary>
	public IReadOnlyList<string> Categories { get; } = new[] { "温度", "長さ", "重さ" };

	/// <summary>
	/// 指定したカテゴリで選択可能な単位の一覧を返す。
	/// </summary>
	/// <param name="category"><see cref="Categories"/> に含まれるカテゴリ名。</param>
	public IReadOnlyList<string> GetUnits(string category) => _converters[category].Units;

	/// <summary>
	/// 指定したカテゴリのコンバータに変換処理を委譲する。
	/// </summary>
	/// <param name="category"><see cref="Categories"/> に含まれるカテゴリ名。</param>
	/// <param name="value">変換元の値。</param>
	/// <param name="fromUnit">変換元の単位。</param>
	/// <param name="toUnit">変換先の単位。</param>
	public decimal Convert(string category, decimal value, string fromUnit, string toUnit) =>
		_converters[category].Convert(value, fromUnit, toUnit);
}
