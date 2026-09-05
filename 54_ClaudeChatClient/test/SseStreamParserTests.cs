using ClaudeChatClient.Services;

namespace ClaudeChatClient.Tests;

public class SseStreamParserTests
{
	private static async Task<List<string>> CollectAsync(string sseText)
	{
		using var reader = new StringReader(sseText);
		var results = new List<string>();
		await foreach (var text in SseStreamParser.ParseTextDeltasAsync(reader))
		{
			results.Add(text);
		}

		return results;
	}

	/// <summary>
	/// パス条件: 単一チャンクのcontent_block_deltaからテキスト差分を抽出できること。
	/// </summary>
	[Fact]
	public async Task ParseTextDeltasAsync_SingleChunk_ExtractsText()
	{
		const string sse = """
			event: content_block_delta
			data: {"type":"content_block_delta","index":0,"delta":{"type":"text_delta","text":"Hello"}}

			event: message_stop
			data: {"type":"message_stop"}

			""";

		var results = await CollectAsync(sse);

		Assert.Equal(["Hello"], results);
	}

	/// <summary>
	/// パス条件: 複数のcontent_block_deltaイベントが順番に結合され、複数のテキスト差分として得られること。
	/// </summary>
	[Fact]
	public async Task ParseTextDeltasAsync_MultipleChunks_ExtractsAllInOrder()
	{
		const string sse = """
			event: content_block_delta
			data: {"type":"content_block_delta","index":0,"delta":{"type":"text_delta","text":"Hello"}}

			event: content_block_delta
			data: {"type":"content_block_delta","index":0,"delta":{"type":"text_delta","text":" world"}}

			event: message_stop
			data: {"type":"message_stop"}

			""";

		var results = await CollectAsync(sse);

		Assert.Equal(["Hello", " world"], results);
	}

	/// <summary>
	/// パス条件: 1つのdataが複数行にまたがる場合でも改行を挟んで結合しJSONとして解釈できること。
	/// </summary>
	[Fact]
	public async Task ParseTextDeltasAsync_DataSpanningMultipleLines_ParsesCorrectly()
	{
		const string sse = "event: content_block_delta\n" +
			"data: {\"type\":\"content_block_delta\",\"index\":0,\n" +
			"data: \"delta\":{\"type\":\"text_delta\",\"text\":\"Hi\"}}\n" +
			"\n" +
			"event: message_stop\n" +
			"data: {\"type\":\"message_stop\"}\n" +
			"\n";

		var results = await CollectAsync(sse);

		Assert.Equal(["Hi"], results);
	}

	/// <summary>
	/// パス条件: message_stopイベントを受信すると、それ以降のイベントを読まずに列挙が終わること。
	/// </summary>
	[Fact]
	public async Task ParseTextDeltasAsync_MessageStop_EndsEnumeration()
	{
		const string sse = """
			event: message_start
			data: {"type":"message_start"}

			event: content_block_delta
			data: {"type":"content_block_delta","index":0,"delta":{"type":"text_delta","text":"A"}}

			event: message_stop
			data: {"type":"message_stop"}

			event: content_block_delta
			data: {"type":"content_block_delta","index":0,"delta":{"type":"text_delta","text":"B"}}

			""";

		var results = await CollectAsync(sse);

		Assert.Equal(["A"], results);
	}

	/// <summary>
	/// パス条件: エラーイベント受信時に<see cref="ClaudeApiException"/>がスローされること。
	/// </summary>
	[Fact]
	public async Task ParseTextDeltasAsync_ErrorEvent_ThrowsClaudeApiException()
	{
		const string sse = """
			event: error
			data: {"type":"error","error":{"type":"overloaded_error","message":"Overloaded"}}

			""";

		var exception = await Assert.ThrowsAsync<ClaudeApiException>(() => CollectAsync(sse));
		Assert.Contains("Overloaded", exception.Message);
	}
}
