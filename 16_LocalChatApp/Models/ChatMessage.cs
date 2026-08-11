namespace LocalChatApp.Models;

/// <summary>
/// チャットメッセージの発信元。
/// </summary>
public enum MessageSender
{
	/// <summary>自分が送信したメッセージ。</summary>
	Local,

	/// <summary>相手から受信したメッセージ。</summary>
	Remote,

	/// <summary>接続確立・切断等のシステム通知。</summary>
	System,
}

/// <summary>
/// チャット欄に表示する1件のメッセージ。
/// </summary>
public class ChatMessage
{
	/// <summary>発信元。</summary>
	public required MessageSender Sender { get; init; }

	/// <summary>本文。</summary>
	public required string Text { get; init; }

	/// <summary>表示時刻。</summary>
	public required DateTime Timestamp { get; init; }
}
