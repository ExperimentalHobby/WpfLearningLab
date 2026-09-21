namespace ClaudeChatClient.Services;

/// <summary>
/// APIキーレコードの永続化(保存)で発生したエラー(ファイルアクセス権限エラー等)を表す。
/// </summary>
public class ApiKeyStoreException : Exception
{
	/// <summary>
	/// <see cref="ApiKeyStoreException"/>を初期化する。
	/// </summary>
	/// <param name="message">エラーメッセージ。</param>
	/// <param name="innerException">原因となった例外。</param>
	public ApiKeyStoreException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
