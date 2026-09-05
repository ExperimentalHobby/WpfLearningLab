namespace ClaudeChatClient.Models;

/// <summary>
/// 発言者の区分。
/// </summary>
public enum ChatRole
{
	/// <summary>ユーザーからの発言。</summary>
	User,

	/// <summary>アシスタント(Claude)からの発言。</summary>
	Assistant,
}

/// <summary>
/// 会話履歴の1件の発言。
/// </summary>
/// <param name="Role">発言者。</param>
/// <param name="Content">発言内容。</param>
public record ChatMessage(ChatRole Role, string Content)
{
	/// <summary>
	/// 一覧表示用の文字列表現("発言者: 内容")を返す。
	/// </summary>
	public override string ToString() => $"{(Role == ChatRole.User ? "あなた" : "Claude")}: {Content}";
}
