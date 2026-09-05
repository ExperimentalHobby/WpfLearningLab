using System.Runtime.CompilerServices;
using ClaudeChatClient.Models;
using ClaudeChatClient.Services;

namespace ClaudeChatClient.Tests.Fakes;

/// <summary>
/// テスト用の<see cref="IClaudeApiClient"/>フェイク実装。あらかじめ設定したテキスト差分を順に返す。
/// <see cref="UseGateAfterFirstChunk"/>を有効にすると、1件目のチャンクをyieldした直後に
/// <see cref="ReleaseGate"/>が呼ばれるまで確実に停止する。<c>Task.Yield()</c>だけに頼ると
/// 実際に停止するタイミングがスレッドスケジューリング依存になり、CI環境ではキャンセル操作を
/// 挟む前にストリーミングが完了してしまうことがある(タイミング依存の競合状態)ため、
/// テストからキャンセルのタイミングを確実に制御できるようこのゲートを用意した。
/// </summary>
public class FakeClaudeApiClient : IClaudeApiClient
{
	private readonly TaskCompletionSource _waitingAtGate = new();
	private readonly TaskCompletionSource _gate = new();

	public List<string> ChunksToYield { get; set; } = [];

	public IReadOnlyList<ChatMessage>? LastHistory { get; private set; }

	public CancellationToken LastCancellationToken { get; private set; }

	public bool UseGateAfterFirstChunk { get; set; }

	/// <summary>
	/// 1件目のチャンクをyieldし終え、ゲート待機に入るまで完了しないタスク。
	/// </summary>
	public Task WaitUntilWaitingAtGateAsync() => _waitingAtGate.Task;

	/// <summary>
	/// ゲートで待機中の列挙を再開させる。
	/// </summary>
	public void ReleaseGate() => _gate.TrySetResult();

	public async IAsyncEnumerable<string> StreamMessageAsync(
		IReadOnlyList<ChatMessage> history,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		LastHistory = history;
		LastCancellationToken = cancellationToken;

		for (var i = 0; i < ChunksToYield.Count; i++)
		{
			if (i == 1 && UseGateAfterFirstChunk)
			{
				_waitingAtGate.TrySetResult();
				await _gate.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
			}

			cancellationToken.ThrowIfCancellationRequested();
			yield return ChunksToYield[i];
		}
	}
}
