using CustomMarkupExtension.MarkupExtensions;

namespace CustomMarkupExtension.Tests;

/// <summary>
/// <see cref="UnitConverter"/>のテスト。
/// </summary>
public class UnitConverterTests
{
	/// <summary>
	/// パス条件: センチメートルからピクセルに変換すると、96px/2.54cmの比率で変換されること。
	/// </summary>
	[Fact]
	public void Convert_センチメートルからピクセルに変換される()
	{
		var result = UnitConverter.Convert(2.54, UnitOfLength.Centimeter, UnitOfLength.Pixel);
		Assert.Equal(96, result, precision: 6);
	}

	/// <summary>
	/// パス条件: インチからピクセルに変換すると、1インチ=96pxで変換されること。
	/// </summary>
	[Fact]
	public void Convert_インチからピクセルに変換される()
	{
		var result = UnitConverter.Convert(1, UnitOfLength.Inch, UnitOfLength.Pixel);
		Assert.Equal(96, result, precision: 6);
	}

	/// <summary>
	/// パス条件: 同じ単位同士の変換では値が変化しないこと。
	/// </summary>
	[Fact]
	public void Convert_同じ単位同士では値が変化しない()
	{
		var result = UnitConverter.Convert(50, UnitOfLength.Pixel, UnitOfLength.Pixel);
		Assert.Equal(50, result, precision: 6);
	}

	/// <summary>
	/// パス条件: ピクセルからセンチメートルへの変換が、センチメートルからピクセルへの変換の逆変換になること。
	/// </summary>
	[Fact]
	public void Convert_ピクセルとセンチメートルの往復変換で元の値に戻る()
	{
		var pixels = UnitConverter.Convert(10, UnitOfLength.Centimeter, UnitOfLength.Pixel);
		var backToCentimeters = UnitConverter.Convert(pixels, UnitOfLength.Pixel, UnitOfLength.Centimeter);
		Assert.Equal(10, backToCentimeters, precision: 6);
	}
}
