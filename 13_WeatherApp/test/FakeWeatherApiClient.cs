using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.Tests;

/// <summary>
/// <see cref="MainViewModel"/> のテスト用に、実通信を行わない<see cref="IWeatherApiClient"/>実装。
/// 各メソッドの戻り値・例外を差し替え可能にする。
/// </summary>
public class FakeWeatherApiClient : IWeatherApiClient
{
	public GeocodingResult? SearchLocationResult { get; set; }
	public CurrentWeather? CurrentWeatherResult { get; set; }
	public Exception? ExceptionToThrow { get; set; }
	public TaskCompletionSource? SearchLocationGate { get; set; }

	public async Task<GeocodingResult?> SearchLocationAsync(string placeName)
	{
		if (SearchLocationGate is not null)
		{
			await SearchLocationGate.Task;
		}

		if (ExceptionToThrow is not null)
		{
			throw ExceptionToThrow;
		}

		return SearchLocationResult;
	}

	public Task<CurrentWeather> GetCurrentWeatherAsync(double latitude, double longitude)
	{
		if (ExceptionToThrow is not null)
		{
			throw ExceptionToThrow;
		}

		return Task.FromResult(CurrentWeatherResult!);
	}
}
