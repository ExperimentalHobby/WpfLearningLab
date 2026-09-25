using System.Net;

namespace TaskApiWorkbench.Client.Tests;

/// <summary>
/// <see cref="Services.TaskApiClient"/>等のテスト用に、実ネットワーク通信をせず
/// 差し替えたJSON文字列をそのまま返す<see cref="HttpMessageHandler"/>。
/// </summary>
public class FakeHttpMessageHandler : HttpMessageHandler
{
	private readonly HttpStatusCode _statusCode;
	private readonly string _responseBody;
	private readonly Action<HttpRequestMessage, string?>? _onRequest;

	public FakeHttpMessageHandler(string responseBody, HttpStatusCode statusCode = HttpStatusCode.OK, Action<HttpRequestMessage, string?>? onRequest = null)
	{
		_responseBody = responseBody;
		_statusCode = statusCode;
		_onRequest = onRequest;
	}

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		if (_onRequest is not null)
		{
			var body = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
			_onRequest(request, body);
		}

		return new HttpResponseMessage(_statusCode)
		{
			Content = new StringContent(_responseBody),
		};
	}
}
