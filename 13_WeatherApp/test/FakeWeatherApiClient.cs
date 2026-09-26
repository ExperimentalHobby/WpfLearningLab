using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.Tests;

/// <summary>
/// <see cref="WeatherApp.ViewModels.MainViewModel"/> のテスト用に、実通信を行わない<see cref="IWeatherApiClient"/>実装。
/// 各メソッドの戻り値・例外を差し替え可能にする。
/// </summary>
public class FakeWeatherApiClient : IWeatherApiClient
{
	/// <summary><see cref="SearchLocationAsync"/>が返す値(テスト用)。</summary>
	public GeocodingResult? SearchLocationResult { get; set; }

	/// <summary><see cref="GetCurrentWeatherAsync"/>が返す値(テスト用)。</summary>
	public CurrentWeather? CurrentWeatherResult { get; set; }

	/// <summary>設定すると各メソッド呼び出し時にこの例外をスローする(テスト用)。</summary>
	public Exception? ExceptionToThrow { get; set; }

	/// <summary>設定すると<see cref="SearchLocationAsync"/>がこのTaskの完了まで待機する(テスト用)。</summary>
	public TaskCompletionSource? SearchLocationGate { get; set; }

	/// <inheritdoc/>
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

	/// <inheritdoc/>
	public Task<CurrentWeather> GetCurrentWeatherAsync(double latitude, double longitude)
	{
		if (ExceptionToThrow is not null)
		{
			throw ExceptionToThrow;
		}

		return Task.FromResult(CurrentWeatherResult!);
	}
}
