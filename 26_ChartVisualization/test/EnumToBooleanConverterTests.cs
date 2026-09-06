using System.Globalization;
using System.Windows.Data;
using ChartVisualization.Converters;
using ChartVisualization.Models;

namespace ChartVisualization.Tests;

/// <summary>
/// <see cref="EnumToBooleanConverter"/> の単体テスト。
/// </summary>
public class EnumToBooleanConverterTests
{
	/// <summary>
	/// パス条件: 値とパラメーターの文字列表現が一致する場合trueを返すこと
	/// </summary>
	[Fact]
	public void Convert_値とパラメーターが一致する場合trueを返す()
	{
		var converter = new EnumToBooleanConverter();

		var result = converter.Convert(ChartType.Bar, typeof(bool), "Bar", CultureInfo.InvariantCulture);

		Assert.Equal(true, result);
	}

	/// <summary>
	/// パス条件: ConvertBackでtrueが渡された場合、パラメーターに対応する列挙値を返すこと
	/// </summary>
	[Fact]
	public void ConvertBack_trueの場合パラメーターに対応する列挙値を返す()
	{
		var converter = new EnumToBooleanConverter();

		var result = converter.ConvertBack(true, typeof(ChartType), "Pie", CultureInfo.InvariantCulture);

		Assert.Equal(ChartType.Pie, result);
	}

	/// <summary>
	/// パス条件: ConvertBackでパラメーターが列挙値として不正な場合、例外を投げずBinding.DoNothingを返すこと
	/// </summary>
	[Fact]
	public void ConvertBack_不正なパラメーターの場合クラッシュせずDoNothingを返す()
	{
		var converter = new EnumToBooleanConverter();

		var result = converter.ConvertBack(true, typeof(ChartType), "存在しない値", CultureInfo.InvariantCulture);

		Assert.Equal(Binding.DoNothing, result);
	}
}
