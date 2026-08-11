using ExchangeRateMonitor.Services;

namespace ExchangeRateMonitor.Tests;

/// <summary>
/// <see cref="FrankfurterExchangeRateApiClient"/> の単体テスト。
/// 実ネットワーク通信はせず、<see cref="FakeHttpMessageHandler"/>でJSON応答を差し替える。
/// </summary>
public class FrankfurterExchangeRateApiClientTests
{
	/// <summary>
	/// パス条件: 正常なレスポンスから為替レートを取得できること
	/// </summary>
	[Fact]
	public async Task GetRateAsync_正常レスポンスからレートを取得できる()
	{
		const string json = """
			{
			  "amount": 1.0,
			  "base": "USD",
			  "date": "2026-08-01",
			  "rates": { "JPY": 157.23 }
			}
			""";
		var httpClient = new HttpClient(new FakeHttpMessageHandler(json));
		var client = new FrankfurterExchangeRateApiClient(httpClient);

		var rate = await client.GetRateAsync("USD", "JPY");

		Assert.Equal(157.23m, rate);
	}

	/// <summary>
	/// パス条件: レスポンスに指定した通貨のレートが含まれない場合、例外がスローされること
	/// </summary>
	[Fact]
	public async Task GetRateAsync_レートが含まれない場合例外がスローされる()
	{
		const string json = """
			{
			  "amount": 1.0,
			  "base": "USD",
			  "date": "2026-08-01",
			  "rates": { "EUR": 0.92 }
			}
			""";
		var httpClient = new HttpClient(new FakeHttpMessageHandler(json));
		var client = new FrankfurterExchangeRateApiClient(httpClient);

		await Assert.ThrowsAsync<InvalidOperationException>(() => client.GetRateAsync("USD", "JPY"));
	}

	/// <summary>
	/// パス条件: HTTPエラーステータスが返された場合、例外がスローされること
	/// </summary>
	[Fact]
	public async Task GetRateAsync_HTTPエラーステータスの場合例外がスローされる()
	{
		var httpClient = new HttpClient(new FakeHttpMessageHandler("Not Found", System.Net.HttpStatusCode.NotFound));
		var client = new FrankfurterExchangeRateApiClient(httpClient);

		await Assert.ThrowsAsync<HttpRequestException>(() => client.GetRateAsync("USD", "XXX"));
	}
}
