using System.Net;

namespace SelfHostedNoteSync.Client.Tests;

/// <summary>
/// <see cref="Services.NoteApiClient"/>等のテスト用に、実ネットワーク通信をせず
/// 差し替えたJSON文字列をそのまま返す<see cref="HttpMessageHandler"/>。
/// </summary>
public class FakeHttpMessageHandler : HttpMessageHandler
{
	private readonly HttpStatusCode _statusCode;
	private readonly string _responseBody;
	private readonly Action<HttpRequestMessage, string?>? _onRequest;

	/// <summary>ハンドラーを初期化する。</summary>
	/// <param name="responseBody">返却するレスポンス本文。</param>
	/// <param name="statusCode">返却するステータスコード。</param>
	/// <param name="onRequest">送信されたリクエストを検証するためのコールバック(省略可)。</param>
	public FakeHttpMessageHandler(string responseBody, HttpStatusCode statusCode = HttpStatusCode.OK, Action<HttpRequestMessage, string?>? onRequest = null)
	{
		_responseBody = responseBody;
		_statusCode = statusCode;
		_onRequest = onRequest;
	}

	/// <inheritdoc/>
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
