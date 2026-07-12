namespace UnitConverter.Tests;

/// <summary>
/// <see cref="UnitConverterEngine"/> のカテゴリ選択・単位一覧・変換委譲に関するテスト。
/// </summary>
public class UnitConverterEngineTests
{
	/// <summary>
	/// パス条件: Categories プロパティが「温度」「長さ」「重さ」の3種類を返すこと。
	/// </summary>
	[Fact]
	public void Categories_ReturnsTemperatureLengthWeight()
	{
		var engine = new UnitConverterEngine();

		Assert.Equal(new[] { "温度", "長さ", "重さ" }, engine.Categories);
	}

	/// <summary>
	/// パス条件: カテゴリ「温度」の単位一覧が摂氏・華氏・ケルビンであること。
	/// </summary>
	[Fact]
	public void GetUnits_Temperature_ReturnsCelsiusFahrenheitKelvin()
	{
		var engine = new UnitConverterEngine();

		var units = engine.GetUnits("温度");

		Assert.Equal(new[] { "摂氏", "華氏", "ケルビン" }, units);
	}

	/// <summary>
	/// パス条件: カテゴリ「長さ」の単位一覧が m/cm/inch/feet であること。
	/// </summary>
	[Fact]
	public void GetUnits_Length_ReturnsMeterCentimeterInchFeet()
	{
		var engine = new UnitConverterEngine();

		var units = engine.GetUnits("長さ");

		Assert.Equal(new[] { "m", "cm", "inch", "feet" }, units);
	}

	/// <summary>
	/// パス条件: カテゴリ「重さ」の単位一覧が kg/g/lb であること。
	/// </summary>
	[Fact]
	public void GetUnits_Weight_ReturnsKilogramGramPound()
	{
		var engine = new UnitConverterEngine();

		var units = engine.GetUnits("重さ");

		Assert.Equal(new[] { "kg", "g", "lb" }, units);
	}

	/// <summary>
	/// パス条件: カテゴリ「温度」を指定すると TemperatureConverter に委譲され、正しく変換されること。
	/// </summary>
	[Fact]
	public void Convert_Temperature_DelegatesToTemperatureConverter()
	{
		var engine = new UnitConverterEngine();

		var result = engine.Convert("温度", 0m, "摂氏", "華氏");

		Assert.Equal(32m, result);
	}

	/// <summary>
	/// パス条件: カテゴリ「長さ」を指定すると LengthConverter に委譲され、正しく変換されること。
	/// </summary>
	[Fact]
	public void Convert_Length_DelegatesToLengthConverter()
	{
		var engine = new UnitConverterEngine();

		var result = engine.Convert("長さ", 1m, "m", "cm");

		Assert.Equal(100m, result);
	}

	/// <summary>
	/// パス条件: カテゴリ「重さ」を指定すると WeightConverter に委譲され、正しく変換されること。
	/// </summary>
	[Fact]
	public void Convert_Weight_DelegatesToWeightConverter()
	{
		var engine = new UnitConverterEngine();

		var result = engine.Convert("重さ", 1m, "kg", "g");

		Assert.Equal(1000m, result);
	}
}
