using WeatherApp.Models;

namespace WeatherApp.Services;

/// <summary>
/// 天気情報の取得を担うクライアントの抽象。
/// </summary>
public interface IWeatherApiClient
{
	/// <summary>
	/// 地名から緯度経度を解決する。該当する地名が見つからない場合は <see langword="null"/> を返す。
	/// </summary>
	Task<GeocodingResult?> SearchLocationAsync(string placeName);

	/// <summary>
	/// 指定した緯度経度の現在の天気を取得する。
	/// </summary>
	Task<CurrentWeather> GetCurrentWeatherAsync(double latitude, double longitude);
}
