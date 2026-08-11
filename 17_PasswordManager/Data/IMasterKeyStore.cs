namespace PasswordManager.Data;

/// <summary>
/// マスターパスワードの鍵導出用ソルトと検証用値の永続化を担う抽象。
/// </summary>
public interface IMasterKeyStore
{
	/// <summary>
	/// マスターパスワードが初期設定済みかどうか。
	/// </summary>
	bool IsInitialized();

	/// <summary>
	/// 鍵導出用のソルトを取得する。未初期化の場合は例外を送出する。
	/// </summary>
	byte[] GetSalt();

	/// <summary>
	/// マスターパスワード正誤判定用の検証用値(暗号化済み)を取得する。未初期化の場合は例外を送出する。
	/// </summary>
	string GetVerificationValue();

	/// <summary>
	/// ソルトと検証用値を保存し、初期設定済みの状態にする。
	/// </summary>
	/// <param name="salt">鍵導出用のソルト。</param>
	/// <param name="verificationValue">既知の文字列を導出鍵で暗号化した検証用値。</param>
	void Initialize(byte[] salt, string verificationValue);
}
