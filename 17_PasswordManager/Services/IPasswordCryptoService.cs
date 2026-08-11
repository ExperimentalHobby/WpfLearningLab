namespace PasswordManager.Services;

/// <summary>
/// マスターパスワードからの鍵導出、およびその鍵によるパスワードの暗号化・復号を担う抽象。
/// </summary>
public interface IPasswordCryptoService
{
	/// <summary>
	/// 鍵導出に使うランダムなソルトを生成する。
	/// </summary>
	byte[] GenerateSalt();

	/// <summary>
	/// マスターパスワードとソルトから暗号鍵を導出する。同じ入力からは常に同じ鍵が得られる。
	/// </summary>
	/// <param name="masterPassword">マスターパスワード。</param>
	/// <param name="salt">鍵導出用のソルト。</param>
	byte[] DeriveKey(string masterPassword, byte[] salt);

	/// <summary>
	/// 平文を鍵で暗号化し、Base64文字列として返す。
	/// </summary>
	/// <param name="plainText">暗号化する平文。</param>
	/// <param name="key">暗号鍵。</param>
	string Encrypt(string plainText, byte[] key);

	/// <summary>
	/// <see cref="Encrypt"/> で暗号化された文字列を鍵で復号する。
	/// </summary>
	/// <param name="cipherText">暗号化された文字列(Base64)。</param>
	/// <param name="key">復号鍵。</param>
	/// <exception cref="System.Security.Cryptography.CryptographicException">鍵が誤っている等で復号に失敗した場合。</exception>
	string Decrypt(string cipherText, byte[] key);
}
