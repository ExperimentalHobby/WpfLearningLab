using WeatherApp.Services;

namespace WeatherApp.Tests;

/// <summary>
/// <see cref="OpenMeteoWeatherApiClient"/> の単体テスト。
/// 実ネットワーク通信はせず、<see cref="FakeHttpMessageHandler"/>でJSON応答を差し替える。
/// </summary>
public class OpenMeteoWeatherApiClientTests
{
	/// <summary>
	/// パス条件: 正常なジオコーディングレスポンスから地名・緯度経度を取得できること
	/// </summary>
	[Fact]
	public async Task SearchLocationAsync_正常レスポンスから地名と緯度経度を取得できる()
	{
		const string json = """
			{
			  "results": [
			    { "id": 1850147, "name": "Tokyo", "latitude": 35.6895, "longitude": 139.69171, "country": "Japan" }
			  ]
			}
			""";
		var httpClient = new HttpClient(new FakeHttpMessageHandler(json));
		var client = new OpenMeteoWeatherApiClient(httpClient);

		var result = await client.SearchLocationAsync("Tokyo");

		Assert.NotNull(result);
		Assert.Equal("Tokyo", result!.Name);
		Assert.Equal(35.6895, result.Latitude);
		Assert.Equal(139.69171, result.Longitude);
	}

	/// <summary>
	/// パス条件: 該当する地名が見つからない場合(resultsフィールドが無い場合)nullを返すこと
	/// </summary>
	[Fact]
	public async Task SearchLocationAsync_該当地名なしの場合nullを返す()
	{
		const string json = "{ \"generationtime_ms\": 0.5 }";
		var httpClient = new HttpClient(new FakeHttpMessageHandler(json));
		var client = new OpenMeteoWeatherApiClient(httpClient);

		var result = await client.SearchLocationAsync("存在しない地名");

		Assert.Null(result);
	}

	/// <summary>
	/// パス条件: 正常な予報レスポンスから気温・湿度・天候コード・風速を取得できること
	/// </summary>
	[Fact]
	public async Task GetCurrentWeatherAsync_正常レスポンスから気温等を取得できる()
	{
		const string json = """
			{
			  "current": {
			    "time": "2026-08-11T12:00",
			    "temperature_2m": 30.5,
			    "relative_humidity_2m": 65,
			    "weather_code": 3,
			    "wind_speed_10m": 12.3
			  }
			}
			""";
		var httpClient = new HttpClient(new FakeHttpMessageHandler(json));
		var client = new OpenMeteoWeatherApiClient(httpClient);

		var result = await client.GetCurrentWeatherAsync(35.6895, 139.69171);

		Assert.Equal(30.5, result.Temperature);
		Assert.Equal(65, result.Humidity);
		Assert.Equal(3, result.WeatherCode);
		Assert.Equal(12.3, result.WindSpeed);
	}
}
