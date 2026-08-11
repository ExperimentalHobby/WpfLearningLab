namespace WeatherApp.Services;

/// <summary>
/// Open-Meteoが返すWMO(世界気象機関)天候コードを、日本語の天候名・絵文字アイコンに変換する。
/// </summary>
public static class WeatherCodeMapper
{
	/// <summary>
	/// 天候コードを日本語の天候名に変換する。未知のコードの場合は「不明」を返す。
	/// </summary>
	public static string ToDescription(int weatherCode) => weatherCode switch
	{
		0 => "快晴",
		1 => "晴れ",
		2 => "一部曇り",
		3 => "曇り",
		45 or 48 => "霧",
		51 or 53 or 55 => "霧雨",
		56 or 57 => "着氷性の霧雨",
		61 or 63 or 65 => "雨",
		66 or 67 => "着氷性の雨",
		71 or 73 or 75 => "雪",
		77 => "霧雪",
		80 or 81 or 82 => "にわか雨",
		85 or 86 => "にわか雪",
		95 => "雷雨",
		96 or 99 => "雹を伴う雷雨",
		_ => "不明",
	};

	/// <summary>
	/// 天候コードを絵文字アイコンに変換する。未知のコードの場合は「❓」を返す。
	/// </summary>
	public static string ToIcon(int weatherCode) => weatherCode switch
	{
		0 => "☀️",
		1 => "🌤️",
		2 => "⛅",
		3 => "☁️",
		45 or 48 => "🌫️",
		51 or 53 or 55 or 56 or 57 => "🌦️",
		61 or 63 or 65 or 66 or 67 or 80 or 81 or 82 => "🌧️",
		71 or 73 or 75 or 77 or 85 or 86 => "❄️",
		95 or 96 or 99 => "⛈️",
		_ => "❓",
	};
}
