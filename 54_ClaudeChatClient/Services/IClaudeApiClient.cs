using ClaudeChatClient.Models;

namespace ClaudeChatClient.Services;

/// <summary>
/// Claude API(Messages API)とのストリーミング通信を担う抽象。
/// </summary>
public interface IClaudeApiClient
{
	/// <summary>
	/// 会話履歴を送信し、ストリーミング応答のテキスト差分を順に列挙する。
	/// </summary>
	/// <param name="history">これまでの会話履歴(最後の要素が最新のユーザー発言)。</param>
	/// <param name="cancellationToken">キャンセル用トークン。</param>
	/// <exception cref="ClaudeApiException">認証エラー・レート制限等でAPI呼び出しが失敗した場合。</exception>
	IAsyncEnumerable<string> StreamMessageAsync(
		IReadOnlyList<ChatMessage> history, CancellationToken cancellationToken = default);
}
