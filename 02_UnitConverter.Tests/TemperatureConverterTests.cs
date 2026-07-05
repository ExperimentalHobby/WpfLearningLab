namespace UnitConverter.Tests;

/// <summary>
/// <see cref="TemperatureConverter"/> の摂氏・華氏・ケルビン相互変換に関するテスト。
/// </summary>
public class TemperatureConverterTests
{
	/// <summary>
	/// パス条件: 摂氏 0度は華氏 32度になること。
	/// </summary>
	[Fact]
	public void Convert_CelsiusToFahrenheit_ZeroCelsiusIs32Fahrenheit()
	{
		var converter = new TemperatureConverter();

		var result = converter.Convert(0m, "摂氏", "華氏");

		Assert.Equal(32m, result);
	}

	/// <summary>
	/// パス条件: 摂氏 100度は華氏 212度になること。
	/// </summary>
	[Fact]
	public void Convert_CelsiusToFahrenheit_HundredCelsiusIs212Fahrenheit()
	{
		var converter = new TemperatureConverter();

		var result = converter.Convert(100m, "摂氏", "華氏");

		Assert.Equal(212m, result);
	}

	/// <summary>
	/// パス条件: 摂氏 0度はケルビン 273.15Kになること。
	/// </summary>
	[Fact]
	public void Convert_CelsiusToKelvin_ZeroCelsiusIs27315Kelvin()
	{
		var converter = new TemperatureConverter();

		var result = converter.Convert(0m, "摂氏", "ケルビン");

		Assert.Equal(273.15m, result);
	}

	/// <summary>
	/// パス条件: 華氏 32度は摂氏 0度になること。
	/// </summary>
	[Fact]
	public void Convert_FahrenheitToCelsius_32FahrenheitIsZeroCelsius()
	{
		var converter = new TemperatureConverter();

		var result = converter.Convert(32m, "華氏", "摂氏");

		Assert.Equal(0m, result);
	}

	/// <summary>
	/// パス条件: ケルビン 273.15Kは摂氏 0度になること。
	/// </summary>
	[Fact]
	public void Convert_KelvinToCelsius_27315KelvinIsZeroCelsius()
	{
		var converter = new TemperatureConverter();

		var result = converter.Convert(273.15m, "ケルビン", "摂氏");

		Assert.Equal(0m, result);
	}

	/// <summary>
	/// パス条件: 同じ単位同士の変換では値が変化しないこと。
	/// </summary>
	[Fact]
	public void Convert_SameUnit_ReturnsSameValue()
	{
		var converter = new TemperatureConverter();

		var result = converter.Convert(25m, "摂氏", "摂氏");

		Assert.Equal(25m, result);
	}

	/// <summary>
	/// パス条件: Units プロパティが摂氏・華氏・ケルビンの3種類を返すこと。
	/// </summary>
	[Fact]
	public void Units_ReturnsCelsiusFahrenheitKelvin()
	{
		var converter = new TemperatureConverter();

		Assert.Equal(new[] { "摂氏", "華氏", "ケルビン" }, converter.Units);
	}
}
