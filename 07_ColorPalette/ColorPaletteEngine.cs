using System.Globalization;

namespace ColorPalette;

/// <summary>
/// RGB値とHEXコード文字列(#RRGGBB形式)の相互変換を行うエンジン。
/// </summary>
public class ColorPaletteEngine
{
	/// <summary>
	/// RGB値をHEXコード文字列(#RRGGBB形式)に変換する。
	/// </summary>
	/// <param name="r">赤成分(0〜255)。</param>
	/// <param name="g">緑成分(0〜255)。</param>
	/// <param name="b">青成分(0〜255)。</param>
	public string ToHex(byte r, byte g, byte b)
	{
		return $"#{r:X2}{g:X2}{b:X2}";
	}

	/// <summary>
	/// HEXコード文字列(#RRGGBBまたはRRGGBB形式、大文字小文字を許容)をRGB値に変換する。
	/// </summary>
	/// <param name="hex">検証対象のHEXコード文字列。</param>
	/// <param name="r">変換できた場合の赤成分。</param>
	/// <param name="g">変換できた場合の緑成分。</param>
	/// <param name="b">変換できた場合の青成分。</param>
	/// <returns>変換できた場合は true、不正な形式の場合は false。</returns>
	public bool TryParseHex(string hex, out byte r, out byte g, out byte b)
	{
		r = g = b = 0;

		var text = hex.StartsWith('#') ? hex[1..] : hex;

		if (text.Length != 6)
		{
			return false;
		}

		if (!byte.TryParse(text.AsSpan(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out r))
		{
			return false;
		}

		if (!byte.TryParse(text.AsSpan(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out g))
		{
			return false;
		}

		if (!byte.TryParse(text.AsSpan(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out b))
		{
			return false;
		}

		return true;
	}
}
