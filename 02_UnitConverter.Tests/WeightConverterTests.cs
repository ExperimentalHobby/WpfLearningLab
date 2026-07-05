namespace UnitConverter.Tests;

/// <summary>
/// <see cref="WeightConverter"/> の kg/g/lb 相互変換に関するテスト。
/// </summary>
public class WeightConverterTests
{
	/// <summary>
	/// パス条件: 1kgは1000gになること。
	/// </summary>
	[Fact]
	public void Convert_KilogramToGram_OneKilogramIs1000Gram()
	{
		var converter = new WeightConverter();

		var result = converter.Convert(1m, "kg", "g");

		Assert.Equal(1000m, result);
	}

	/// <summary>
	/// パス条件: 1000gは1kgになること。
	/// </summary>
	[Fact]
	public void Convert_GramToKilogram_1000GramIsOneKilogram()
	{
		var converter = new WeightConverter();

		var result = converter.Convert(1000m, "g", "kg");

		Assert.Equal(1m, result);
	}

	/// <summary>
	/// パス条件: 1kgは約2.20462lbになること。
	/// </summary>
	[Fact]
	public void Convert_KilogramToPound_OneKilogramIsApproximately220462Pound()
	{
		var converter = new WeightConverter();

		var result = converter.Convert(1m, "kg", "lb");

		Assert.Equal(2.2046226m, Math.Round(result, 7));
	}

	/// <summary>
	/// パス条件: 同じ単位同士の変換では値が変化しないこと。
	/// </summary>
	[Fact]
	public void Convert_SameUnit_ReturnsSameValue()
	{
		var converter = new WeightConverter();

		var result = converter.Convert(3m, "kg", "kg");

		Assert.Equal(3m, result);
	}

	/// <summary>
	/// パス条件: Units プロパティが kg/g/lb の3種類を返すこと。
	/// </summary>
	[Fact]
	public void Units_ReturnsKilogramGramPound()
	{
		var converter = new WeightConverter();

		Assert.Equal(new[] { "kg", "g", "lb" }, converter.Units);
	}
}
