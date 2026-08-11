using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using WeatherApp.Models;

namespace WeatherApp.Services;

/// <summary>
/// Open-Meteo(APIキー不要)を使って地名解決・現在の天気取得を行うクライアント。
/// </summary>
public class OpenMeteoWeatherApiClient : IWeatherApiClient
{
	private const string GeocodingBaseUrl = "https://geocoding-api.open-meteo.com/v1/search";
	private const string ForecastBaseUrl = "https://api.open-meteo.com/v1/forecast";

	private readonly HttpClient _httpClient;

	/// <summary>
	/// クライアントを初期化する。
	/// </summary>
	/// <param name="httpClient">API呼び出しに使う<see cref="HttpClient"/>。テスト時は差し替え可能。</param>
	public OpenMeteoWeatherApiClient(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	/// <inheritdoc/>
	public async Task<GeocodingResult?> SearchLocationAsync(string placeName)
	{
		var url = $"{GeocodingBaseUrl}?name={Uri.EscapeDataString(placeName)}&count=1&language=ja&format=json";
		using var stream = await _httpClient.GetStreamAsync(url);
		using var document = await JsonDocument.ParseAsync(stream);

		if (!document.RootElement.TryGetProperty("results", out var results) || results.GetArrayLength() == 0)
		{
			return null;
		}

		var first = results[0];
		return new GeocodingResult
		{
			Name = first.GetProperty("name").GetString() ?? placeName,
			Latitude = first.GetProperty("latitude").GetDouble(),
			Longitude = first.GetProperty("longitude").GetDouble(),
		};
	}

	/// <inheritdoc/>
	public async Task<CurrentWeather> GetCurrentWeatherAsync(double latitude, double longitude)
	{
		var url = $"{ForecastBaseUrl}?latitude={latitude.ToString(CultureInfo.InvariantCulture)}" +
			$"&longitude={longitude.ToString(CultureInfo.InvariantCulture)}" +
			"&current=temperature_2m,relative_humidity_2m,weather_code,wind_speed_10m&timezone=auto";
		using var stream = await _httpClient.GetStreamAsync(url);
		using var document = await JsonDocument.ParseAsync(stream);

		var current = document.RootElement.GetProperty("current");
		return new CurrentWeather
		{
			Temperature = current.GetProperty("temperature_2m").GetDouble(),
			Humidity = current.GetProperty("relative_humidity_2m").GetDouble(),
			WeatherCode = current.GetProperty("weather_code").GetInt32(),
			WindSpeed = current.GetProperty("wind_speed_10m").GetDouble(),
		};
	}
}
