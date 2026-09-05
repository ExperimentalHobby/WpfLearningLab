namespace ClaudeChatClient.Services;

/// <summary>
/// Claude APIとの通信で発生したエラー(認証エラー・レート制限・サーバー側エラー等)を表す。
/// </summary>
public class ClaudeApiException : Exception
{
	/// <summary>
	/// <see cref="ClaudeApiException"/>を初期化する。
	/// </summary>
	/// <param name="message">エラーメッセージ。</param>
	public ClaudeApiException(string message) : base(message)
	{
	}

	/// <summary>
	/// <see cref="ClaudeApiException"/>を初期化する。
	/// </summary>
	/// <param name="message">エラーメッセージ。</param>
	/// <param name="innerException">原因となった例外。</param>
	public ClaudeApiException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
