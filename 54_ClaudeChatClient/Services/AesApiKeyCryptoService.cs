using System.Security.Cryptography;
using System.Text;

namespace ClaudeChatClient.Services;

/// <summary>
/// AES-GCM(認証付き暗号)とPBKDF2による鍵導出でAPIキーを暗号化・復号する実装。
/// (17_PasswordManagerの<c>AesPasswordCryptoService</c>と同等のロジック)
/// </summary>
public class AesApiKeyCryptoService : IApiKeyCryptoService
{
	private const int SaltSizeBytes = 16;
	private const int KeySizeBytes = 32;
	private const int Pbkdf2Iterations = 100_000;

	/// <summary>GCMのnonceサイズ(バイト)。.NETの<see cref="AesGcm"/>がサポートする唯一のサイズ。</summary>
	private static readonly int NonceSizeBytes = AesGcm.NonceByteSizes.MaxSize;

	/// <summary>認証タグのサイズ(バイト)。最大サイズを使い、改ざん検知の安全余裕を最大にする。</summary>
	private static readonly int TagSizeBytes = AesGcm.TagByteSizes.MaxSize;

	/// <inheritdoc/>
	public byte[] GenerateSalt() => RandomNumberGenerator.GetBytes(SaltSizeBytes);

	/// <inheritdoc/>
	public byte[] DeriveKey(string masterPassword, byte[] salt) =>
		Rfc2898DeriveBytes.Pbkdf2(
			Encoding.UTF8.GetBytes(masterPassword),
			salt,
			Pbkdf2Iterations,
			HashAlgorithmName.SHA256,
			KeySizeBytes);

	/// <inheritdoc/>
	public string Encrypt(string plainText, byte[] key)
	{
		var nonce = RandomNumberGenerator.GetBytes(NonceSizeBytes);
		var plainBytes = Encoding.UTF8.GetBytes(plainText);
		var cipherBytes = new byte[plainBytes.Length];
		var tag = new byte[TagSizeBytes];

		using (var aesGcm = new AesGcm(key, TagSizeBytes))
		{
			aesGcm.Encrypt(nonce, plainBytes, cipherBytes, tag);
		}

		// Nonce+認証タグ(復号に必要、秘匿不要)を先頭に付与してからBase64化する。
		var result = new byte[nonce.Length + tag.Length + cipherBytes.Length];
		nonce.CopyTo(result, 0);
		tag.CopyTo(result, nonce.Length);
		cipherBytes.CopyTo(result, nonce.Length + tag.Length);

		return Convert.ToBase64String(result);
	}

	/// <inheritdoc/>
	/// <exception cref="CryptographicException">
	/// 鍵が誤っている、または暗号文が改ざんされている場合、認証タグの検証に失敗し必ずスローされる。
	/// </exception>
	public string Decrypt(string cipherText, byte[] key)
	{
		var data = Convert.FromBase64String(cipherText);

		var nonce = data.AsSpan(0, NonceSizeBytes);
		var tag = data.AsSpan(NonceSizeBytes, TagSizeBytes);
		var cipherBytes = data.AsSpan(NonceSizeBytes + TagSizeBytes);
		var plainBytes = new byte[cipherBytes.Length];

		using (var aesGcm = new AesGcm(key, TagSizeBytes))
		{
			aesGcm.Decrypt(nonce, cipherBytes, tag, plainBytes);
		}

		return Encoding.UTF8.GetString(plainBytes);
	}
}
