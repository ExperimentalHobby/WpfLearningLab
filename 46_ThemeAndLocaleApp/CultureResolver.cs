using System.Globalization;

namespace ThemeAndLocaleApp;

/// <summary>
/// カルチャコードの文字列から<see cref="CultureInfo"/>を安全に解決するロジック。
/// </summary>
public static class CultureResolver
{
	/// <summary>
	/// 指定したカルチャコードから<see cref="CultureInfo"/>を解決する。
	/// 設定ファイルの破損等で無効なカルチャコードが渡された場合は例外にせず、
	/// <paramref name="fallback"/>のカルチャにフォールバックする。
	/// </summary>
	/// <param name="culture">解決したいカルチャコード(例: "ja"、"en")。</param>
	/// <param name="fallback">解決に失敗した場合に使うフォールバックのカルチャコード。既定は"ja"。</param>
	public static CultureInfo Resolve(string culture, string fallback = "ja")
	{
		try
		{
			return new CultureInfo(culture);
		}
		catch (CultureNotFoundException)
		{
			return new CultureInfo(fallback);
		}
	}
}
