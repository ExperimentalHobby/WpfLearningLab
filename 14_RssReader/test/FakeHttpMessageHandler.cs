using System.Net;

namespace RssReader.Tests;

/// <summary>
/// <see cref="RssFeedClient"/>等のテスト用に、実ネットワーク通信をせず
/// 差し替えた応答本文をそのまま返す<see cref="HttpMessageHandler"/>。
/// </summary>
public class FakeHttpMessageHandler : HttpMessageHandler
{
	private readonly HttpStatusCode _statusCode;
	private readonly string _responseBody;

	public FakeHttpMessageHandler(string responseBody, HttpStatusCode statusCode = HttpStatusCode.OK)
	{
		_responseBody = responseBody;
		_statusCode = statusCode;
	}

	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var response = new HttpResponseMessage(_statusCode)
		{
			Content = new StringContent(_responseBody),
		};
		return Task.FromResult(response);
	}
}
