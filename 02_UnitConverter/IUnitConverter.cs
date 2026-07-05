namespace UnitConverter;

/// <summary>
/// 特定カテゴリ(温度・長さ・重さなど)の単位変換を行うコンバータの共通インターフェース。
/// </summary>
public interface IUnitConverter
{
	/// <summary>
	/// このカテゴリで選択可能な単位の一覧。
	/// </summary>
	IReadOnlyList<string> Units { get; }

	/// <summary>
	/// 値を fromUnit から toUnit へ変換する。
	/// </summary>
	/// <param name="value">変換元の値。</param>
	/// <param name="fromUnit">変換元の単位。<see cref="Units"/> に含まれる値であること。</param>
	/// <param name="toUnit">変換先の単位。<see cref="Units"/> に含まれる値であること。</param>
	decimal Convert(decimal value, string fromUnit, string toUnit);
}
