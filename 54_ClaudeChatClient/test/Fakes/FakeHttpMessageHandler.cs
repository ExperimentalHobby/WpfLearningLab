using System.Net;

namespace ClaudeChatClient.Tests.Fakes;

/// <summary>
/// テスト用の<see cref="HttpMessageHandler"/>フェイク実装。実際の通信は行わず、
/// あらかじめ設定した応答を返す。
/// </summary>
public class FakeHttpMessageHandler : HttpMessageHandler
{
	public HttpStatusCode StatusCodeToReturn { get; set; } = HttpStatusCode.OK;

	public string ContentToReturn { get; set; } = string.Empty;

	public HttpRequestMessage? LastRequest { get; private set; }

	public string? LastRequestBody { get; private set; }

	protected override async Task<HttpResponseMessage> SendAsync(
		HttpRequestMessage request, CancellationToken cancellationToken)
	{
		LastRequest = request;
		if (request.Content is not null)
		{
			LastRequestBody = await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
		}

		cancellationToken.ThrowIfCancellationRequested();

		return new HttpResponseMessage(StatusCodeToReturn)
		{
			Content = new StringContent(ContentToReturn),
		};
	}
}
