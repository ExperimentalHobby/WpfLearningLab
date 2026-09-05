namespace ClaudeChatClient.Services;

/// <summary>
/// 暗号化保存されたAPIキーの永続化レコード。
/// </summary>
/// <param name="Salt">鍵導出用のソルト。</param>
/// <param name="VerificationCipherText">マスターパスワード検証用の暗号文(既知の固定文字列を暗号化したもの)。</param>
/// <param name="EncryptedApiKey">暗号化されたAPIキー。</param>
public record ApiKeyRecord(byte[] Salt, string VerificationCipherText, string EncryptedApiKey);
