using System.Net;
using ClaudeChatClient.Models;
using ClaudeChatClient.Services;
using ClaudeChatClient.Tests.Fakes;

namespace ClaudeChatClient.Tests;

public class ClaudeApiClientTests
{
	private static readonly IReadOnlyList<ChatMessage> SampleHistory =
	[
		new ChatMessage(ChatRole.User, "こんにちは"),
	];

	/// <summary>
	/// パス条件: 擬似SSEレスポンスから期待するテキスト差分列を受信できること。
	/// </summary>
	[Fact]
	public async Task StreamMessageAsync_SuccessfulResponse_YieldsExpectedTextDeltas()
	{
		var handler = new FakeHttpMessageHandler
		{
			StatusCodeToReturn = HttpStatusCode.OK,
			ContentToReturn = """
				event: content_block_delta
				data: {"type":"content_block_delta","index":0,"delta":{"type":"text_delta","text":"こんにちは"}}

				event: content_block_delta
				data: {"type":"content_block_delta","index":0,"delta":{"type":"text_delta","text":"!"}}

				event: message_stop
				data: {"type":"message_stop"}

				""",
		};
		using var httpClient = new HttpClient(handler);
		var client = new ClaudeApiClient(httpClient, "test-api-key");

		var results = new List<string>();
		await foreach (var text in client.StreamMessageAsync(SampleHistory))
		{
			results.Add(text);
		}

		Assert.Equal(["こんにちは", "!"], results);
	}

	/// <summary>
	/// パス条件: 401応答(認証エラー)で<see cref="ClaudeApiException"/>がスローされること。
	/// </summary>
	[Fact]
	public async Task StreamMessageAsync_Unauthorized_ThrowsClaudeApiException()
	{
		var handler = new FakeHttpMessageHandler { StatusCodeToReturn = HttpStatusCode.Unauthorized };
		using var httpClient = new HttpClient(handler);
		var client = new ClaudeApiClient(httpClient, "invalid-key");

		await Assert.ThrowsAsync<ClaudeApiException>(async () =>
		{
			await foreach (var _ in client.StreamMessageAsync(SampleHistory))
			{
			}
		});
	}

	/// <summary>
	/// パス条件: 429応答(レート制限)で<see cref="ClaudeApiException"/>がスローされること。
	/// </summary>
	[Fact]
	public async Task StreamMessageAsync_TooManyRequests_ThrowsClaudeApiException()
	{
		var handler = new FakeHttpMessageHandler { StatusCodeToReturn = HttpStatusCode.TooManyRequests };
		using var httpClient = new HttpClient(handler);
		var client = new ClaudeApiClient(httpClient, "test-api-key");

		await Assert.ThrowsAsync<ClaudeApiException>(async () =>
		{
			await foreach (var _ in client.StreamMessageAsync(SampleHistory))
			{
			}
		});
	}

	/// <summary>
	/// パス条件: 事前にキャンセルされたトークンを渡すと、列挙開始時にキャンセル例外が伝播すること。
	/// </summary>
	[Fact]
	public async Task StreamMessageAsync_CancelledToken_ThrowsOperationCanceledException()
	{
		var handler = new FakeHttpMessageHandler { StatusCodeToReturn = HttpStatusCode.OK };
		using var httpClient = new HttpClient(handler);
		var client = new ClaudeApiClient(httpClient, "test-api-key");
		using var cts = new CancellationTokenSource();
		await cts.CancelAsync();

		await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
		{
			await foreach (var _ in client.StreamMessageAsync(SampleHistory, cts.Token))
			{
			}
		});
	}
}
