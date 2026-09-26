using System.Net;

namespace ExchangeRateMonitor.Tests;

/// <summary>
/// <see cref="Services.FrankfurterExchangeRateApiClient"/>等のテスト用に、実ネットワーク通信をせず
/// 差し替えたJSON文字列をそのまま返す<see cref="HttpMessageHandler"/>。
/// </summary>
public class FakeHttpMessageHandler : HttpMessageHandler
{
	private readonly HttpStatusCode _statusCode;
	private readonly string _responseBody;

	/// <summary>ハンドラーを初期化する。</summary>
	/// <param name="responseBody">返却するレスポンス本文。</param>
	/// <param name="statusCode">返却するステータスコード。</param>
	public FakeHttpMessageHandler(string responseBody, HttpStatusCode statusCode = HttpStatusCode.OK)
	{
		_responseBody = responseBody;
		_statusCode = statusCode;
	}

	/// <inheritdoc/>
	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var response = new HttpResponseMessage(_statusCode)
		{
			Content = new StringContent(_responseBody),
		};
		return Task.FromResult(response);
	}
}
