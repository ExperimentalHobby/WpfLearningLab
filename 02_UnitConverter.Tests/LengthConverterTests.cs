namespace UnitConverter.Tests;

/// <summary>
/// <see cref="LengthConverter"/> の m/cm/inch/feet 相互変換に関するテスト。
/// </summary>
public class LengthConverterTests
{
	/// <summary>
	/// パス条件: 1mは100cmになること。
	/// </summary>
	[Fact]
	public void Convert_MeterToCentimeter_OneMeterIs100Centimeter()
	{
		var converter = new LengthConverter();

		var result = converter.Convert(1m, "m", "cm");

		Assert.Equal(100m, result);
	}

	/// <summary>
	/// パス条件: 100cmは1mになること。
	/// </summary>
	[Fact]
	public void Convert_CentimeterToMeter_100CentimeterIsOneMeter()
	{
		var converter = new LengthConverter();

		var result = converter.Convert(100m, "cm", "m");

		Assert.Equal(1m, result);
	}

	/// <summary>
	/// パス条件: 1inchは2.54cmになること。
	/// </summary>
	[Fact]
	public void Convert_InchToCentimeter_OneInchIs254Centimeter()
	{
		var converter = new LengthConverter();

		var result = converter.Convert(1m, "inch", "cm");

		Assert.Equal(2.54m, result);
	}

	/// <summary>
	/// パス条件: 1feetは12inchになること。
	/// </summary>
	[Fact]
	public void Convert_FeetToInch_OneFeetIs12Inch()
	{
		var converter = new LengthConverter();

		var result = converter.Convert(1m, "feet", "inch");

		Assert.Equal(12m, result);
	}

	/// <summary>
	/// パス条件: 同じ単位同士の変換では値が変化しないこと。
	/// </summary>
	[Fact]
	public void Convert_SameUnit_ReturnsSameValue()
	{
		var converter = new LengthConverter();

		var result = converter.Convert(5m, "m", "m");

		Assert.Equal(5m, result);
	}

	/// <summary>
	/// パス条件: Units プロパティが m/cm/inch/feet の4種類を返すこと。
	/// </summary>
	[Fact]
	public void Units_ReturnsMeterCentimeterInchFeet()
	{
		var converter = new LengthConverter();

		Assert.Equal(new[] { "m", "cm", "inch", "feet" }, converter.Units);
	}
}
