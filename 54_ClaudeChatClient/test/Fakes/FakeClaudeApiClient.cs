using System.Runtime.CompilerServices;
using ClaudeChatClient.Models;
using ClaudeChatClient.Services;

namespace ClaudeChatClient.Tests.Fakes;

/// <summary>
/// テスト用の<see cref="IClaudeApiClient"/>フェイク実装。あらかじめ設定したテキスト差分を順に返す。
/// <see cref="YieldGate"/>が設定されている場合、各チャンクをyieldする前に<c>Task.Yield()</c>で
/// 一旦呼び出し元に制御を返し、キャンセル操作をテストから挟めるようにする。
/// </summary>
public class FakeClaudeApiClient : IClaudeApiClient
{
	public List<string> ChunksToYield { get; set; } = [];

	public IReadOnlyList<ChatMessage>? LastHistory { get; private set; }

	public CancellationToken LastCancellationToken { get; private set; }

	public bool YieldBeforeEachChunk { get; set; }

	public async IAsyncEnumerable<string> StreamMessageAsync(
		IReadOnlyList<ChatMessage> history,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		LastHistory = history;
		LastCancellationToken = cancellationToken;

		foreach (var chunk in ChunksToYield)
		{
			if (YieldBeforeEachChunk)
			{
				await Task.Yield();
			}

			cancellationToken.ThrowIfCancellationRequested();
			yield return chunk;
		}
	}
}
