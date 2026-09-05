namespace ColorPalette.Tests;

/// <summary>
/// <see cref="ColorPaletteEngine"/> のRGB⇔HEX変換に関するテスト。
/// </summary>
public class ColorPaletteEngineTests
{
	/// <summary>
	/// パス条件: 黒(0,0,0)をToHexすると"#000000"を返すこと。
	/// </summary>
	[Fact]
	public void ToHex_Black_ReturnsHexBlack()
	{
		var engine = new ColorPaletteEngine();

		var hex = engine.ToHex(0, 0, 0);

		Assert.Equal("#000000", hex);
	}

	/// <summary>
	/// パス条件: 白(255,255,255)をToHexすると"#FFFFFF"を返すこと。
	/// </summary>
	[Fact]
	public void ToHex_White_ReturnsHexWhite()
	{
		var engine = new ColorPaletteEngine();

		var hex = engine.ToHex(255, 255, 255);

		Assert.Equal("#FFFFFF", hex);
	}

	/// <summary>
	/// パス条件: 混合値(255,128,0)をToHexすると"#FF8000"を返すこと。
	/// </summary>
	[Fact]
	public void ToHex_MixedValues_ReturnsCorrectHex()
	{
		var engine = new ColorPaletteEngine();

		var hex = engine.ToHex(255, 128, 0);

		Assert.Equal("#FF8000", hex);
	}

	/// <summary>
	/// パス条件: "#"ありの正常なHEX文字列をTryParseHexするとtrueが返り、正しいRGB値を得られること。
	/// </summary>
	[Fact]
	public void TryParseHex_ValidWithHash_ReturnsTrueAndCorrectRgb()
	{
		var engine = new ColorPaletteEngine();

		var result = engine.TryParseHex("#FF8000", out var r, out var g, out var b);

		Assert.True(result);
		Assert.Equal(255, r);
		Assert.Equal(128, g);
		Assert.Equal(0, b);
	}

	/// <summary>
	/// パス条件: "#"なしの正常なHEX文字列をTryParseHexするとtrueが返り、正しいRGB値を得られること。
	/// </summary>
	[Fact]
	public void TryParseHex_ValidWithoutHash_ReturnsTrueAndCorrectRgb()
	{
		var engine = new ColorPaletteEngine();

		var result = engine.TryParseHex("FF8000", out var r, out var g, out var b);

		Assert.True(result);
		Assert.Equal(255, r);
		Assert.Equal(128, g);
		Assert.Equal(0, b);
	}

	/// <summary>
	/// パス条件: 小文字のHEX文字列をTryParseHexしてもtrueが返ること(大文字小文字を許容)。
	/// </summary>
	[Fact]
	public void TryParseHex_LowercaseHex_ReturnsTrue()
	{
		var engine = new ColorPaletteEngine();

		var result = engine.TryParseHex("#ff8000", out var r, out var g, out var b);

		Assert.True(result);
		Assert.Equal(255, r);
		Assert.Equal(128, g);
		Assert.Equal(0, b);
	}

	/// <summary>
	/// パス条件: 桁数が短いHEX文字列をTryParseHexするとfalseが返ること。
	/// </summary>
	[Fact]
	public void TryParseHex_TooShort_ReturnsFalse()
	{
		var engine = new ColorPaletteEngine();

		var result = engine.TryParseHex("#FFF", out _, out _, out _);

		Assert.False(result);
	}

	/// <summary>
	/// パス条件: 桁数が長いHEX文字列をTryParseHexするとfalseが返ること。
	/// </summary>
	[Fact]
	public void TryParseHex_TooLong_ReturnsFalse()
	{
		var engine = new ColorPaletteEngine();

		var result = engine.TryParseHex("#FF80001", out _, out _, out _);

		Assert.False(result);
	}

	/// <summary>
	/// パス条件: 16進数以外の文字を含むHEX文字列をTryParseHexするとfalseが返ること。
	/// </summary>
	[Fact]
	public void TryParseHex_NonHexCharacters_ReturnsFalse()
	{
		var engine = new ColorPaletteEngine();

		var result = engine.TryParseHex("#GGGGGG", out _, out _, out _);

		Assert.False(result);
	}

	/// <summary>
	/// パス条件: 空文字をTryParseHexするとfalseが返ること。
	/// </summary>
	[Fact]
	public void TryParseHex_EmptyString_ReturnsFalse()
	{
		var engine = new ColorPaletteEngine();

		var result = engine.TryParseHex(string.Empty, out _, out _, out _);

		Assert.False(result);
	}

	/// <summary>
	/// パス条件: ToHexの結果をTryParseHexでパースすると元のRGB値に戻ること(往復変換)。
	/// </summary>
	[Fact]
	public void RoundTrip_ToHexThenParseHex_ReturnsOriginalValues()
	{
		var engine = new ColorPaletteEngine();

		var hex = engine.ToHex(18, 52, 86);
		var result = engine.TryParseHex(hex, out var r, out var g, out var b);

		Assert.True(result);
		Assert.Equal(18, r);
		Assert.Equal(52, g);
		Assert.Equal(86, b);
	}

	/// <summary>
	/// パス条件: nullをTryParseHexしても例外を投げずfalseが返ること
	/// (BCLのTryParse系メソッドの慣例に合わせ、nullは例外ではなく失敗として扱う)。
	/// </summary>
	[Fact]
	public void TryParseHex_Null_ReturnsFalseWithoutThrowing()
	{
		var engine = new ColorPaletteEngine();

		var result = engine.TryParseHex(null!, out _, out _, out _);

		Assert.False(result);
	}
}
