using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using ClaudeChatClient.Models;

namespace ClaudeChatClient.Services;

/// <summary>
/// <see cref="HttpClient"/>でClaude API(Messages API)にストリーミングリクエストを送信し、
/// SSE応答を<see cref="SseStreamParser"/>で解析してテキスト差分を逐次返す実装。
/// </summary>
public class ClaudeApiClient(HttpClient httpClient, string apiKey) : IClaudeApiClient
{
	private const string ApiUrl = "https://api.anthropic.com/v1/messages";
	private const string ApiVersion = "2023-06-01";
	private const string Model = "claude-sonnet-4-5-20250929";
	private const int MaxTokens = 1024;

	/// <inheritdoc/>
	public async IAsyncEnumerable<string> StreamMessageAsync(
		IReadOnlyList<ChatMessage> history,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var requestBody = new
		{
			model = Model,
			max_tokens = MaxTokens,
			stream = true,
			messages = history.Select(m => new
			{
				role = m.Role == ChatRole.User ? "user" : "assistant",
				content = m.Content,
			}),
		};

		using var request = new HttpRequestMessage(HttpMethod.Post, ApiUrl)
		{
			Content = JsonContent.Create(requestBody),
		};
		request.Headers.Add("x-api-key", apiKey);
		request.Headers.Add("anthropic-version", ApiVersion);

		using var response = await httpClient
			.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
			.ConfigureAwait(false);

		if (!response.IsSuccessStatusCode)
		{
			throw response.StatusCode switch
			{
				HttpStatusCode.Unauthorized =>
					new ClaudeApiException("APIキーが無効です(認証エラー)。設定を確認してください。"),
				HttpStatusCode.TooManyRequests =>
					new ClaudeApiException("レート制限を超えました。しばらく待ってから再試行してください。"),
				_ => new ClaudeApiException($"Claude APIエラー: {(int)response.StatusCode} {response.ReasonPhrase}"),
			};
		}

		var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
		await using (stream.ConfigureAwait(false))
		{
			using var reader = new StreamReader(stream);
			await foreach (var text in SseStreamParser.ParseTextDeltasAsync(reader, cancellationToken)
				.ConfigureAwait(false))
			{
				yield return text;
			}
		}
	}
}
