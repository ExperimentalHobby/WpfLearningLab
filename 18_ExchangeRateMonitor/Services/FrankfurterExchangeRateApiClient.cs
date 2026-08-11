using System.Net.Http;
using System.Text.Json;

namespace ExchangeRateMonitor.Services;

/// <summary>
/// Frankfurter API(ECB公表レート、APIキー不要)を使って為替レートを取得するクライアント。
/// </summary>
public class FrankfurterExchangeRateApiClient : IExchangeRateApiClient
{
	private const string BaseUrl = "https://api.frankfurter.app/latest";

	private readonly HttpClient _httpClient;

	/// <summary>
	/// クライアントを初期化する。
	/// </summary>
	/// <param name="httpClient">API呼び出しに使う<see cref="HttpClient"/>。テスト時は差し替え可能。</param>
	public FrankfurterExchangeRateApiClient(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	/// <inheritdoc/>
	public async Task<decimal> GetRateAsync(string baseCurrency, string quoteCurrency)
	{
		var url = $"{BaseUrl}?from={Uri.EscapeDataString(baseCurrency)}&to={Uri.EscapeDataString(quoteCurrency)}";
		using var response = await _httpClient.GetAsync(url);
		response.EnsureSuccessStatusCode();

		using var stream = await response.Content.ReadAsStreamAsync();
		using var document = await JsonDocument.ParseAsync(stream);

		if (!document.RootElement.TryGetProperty("rates", out var rates) ||
			!rates.TryGetProperty(quoteCurrency, out var rateElement))
		{
			throw new InvalidOperationException($"通貨ペア {baseCurrency}/{quoteCurrency} のレートが取得できませんでした。");
		}

		return rateElement.GetDecimal();
	}
}
