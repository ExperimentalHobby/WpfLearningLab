using System.Net;

namespace ClaudeChatClient.Tests.Fakes;

/// <summary>
/// テスト用の<see cref="HttpMessageHandler"/>フェイク実装。実際の通信は行わず、
/// あらかじめ設定した応答を返す。
/// </summary>
public class FakeHttpMessageHandler : HttpMessageHandler
{
	/// <summary>返却するステータスコード(テスト用)。</summary>
	public HttpStatusCode StatusCodeToReturn { get; set; } = HttpStatusCode.OK;

	/// <summary>返却するレスポンス本文(テスト用)。</summary>
	public string ContentToReturn { get; set; } = string.Empty;

	/// <summary>直近に送信されたリクエスト(テスト用)。</summary>
	public HttpRequestMessage? LastRequest { get; private set; }

	/// <summary>直近に送信されたリクエスト本文(テスト用)。</summary>
	public string? LastRequestBody { get; private set; }

	/// <inheritdoc/>
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
