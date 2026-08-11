namespace ExchangeRateMonitor.Services;

/// <summary>
/// 為替レート取得を担うクライアントの抽象。
/// </summary>
public interface IExchangeRateApiClient
{
	/// <summary>
	/// 指定した通貨ペアの為替レート(1<paramref name="baseCurrency"/>あたりの<paramref name="quoteCurrency"/>建て金額)を取得する。
	/// </summary>
	/// <param name="baseCurrency">基軸通貨コード(例: "USD")。</param>
	/// <param name="quoteCurrency">決済通貨コード(例: "JPY")。</param>
	/// <exception cref="System.Net.Http.HttpRequestException">通信エラーが発生した場合。</exception>
	/// <exception cref="System.Text.Json.JsonException">レスポンスの解析に失敗した場合。</exception>
	/// <exception cref="InvalidOperationException">指定した通貨ペアのレートがレスポンスに含まれない場合。</exception>
	Task<decimal> GetRateAsync(string baseCurrency, string quoteCurrency);
}
