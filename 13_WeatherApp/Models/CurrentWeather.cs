namespace WeatherApp.Models;

/// <summary>
/// 現在の天気情報。
/// </summary>
public class CurrentWeather
{
	/// <summary>気温(摂氏)。</summary>
	public required double Temperature { get; init; }

	/// <summary>相対湿度(%)。</summary>
	public required double Humidity { get; init; }

	/// <summary>WMO天候コード。<see cref="Services.WeatherCodeMapper"/>で表示用に変換する。</summary>
	public required int WeatherCode { get; init; }

	/// <summary>風速(km/h)。</summary>
	public required double WindSpeed { get; init; }
}
