using System.Security.Cryptography;
using System.Text;

namespace PasswordManager.Services;

/// <summary>
/// AES(CBCモード)とPBKDF2による鍵導出でパスワードを暗号化・復号する実装。
/// </summary>
public class AesPasswordCryptoService : IPasswordCryptoService
{
	private const int SaltSizeBytes = 16;
	private const int KeySizeBytes = 32;
	private const int Pbkdf2Iterations = 100_000;

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
		using var aes = Aes.Create();
		aes.Key = key;
		aes.GenerateIV();

		using var encryptor = aes.CreateEncryptor();
		var plainBytes = Encoding.UTF8.GetBytes(plainText);
		var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

		// IV(復号に必要、秘匿不要)を先頭に付与してからBase64化する。
		var result = new byte[aes.IV.Length + cipherBytes.Length];
		aes.IV.CopyTo(result, 0);
		cipherBytes.CopyTo(result, aes.IV.Length);

		return Convert.ToBase64String(result);
	}

	/// <inheritdoc/>
	public string Decrypt(string cipherText, byte[] key)
	{
		var data = Convert.FromBase64String(cipherText);

		using var aes = Aes.Create();
		aes.Key = key;
		aes.IV = data[..aes.IV.Length];

		using var decryptor = aes.CreateDecryptor();
		var ivLength = aes.IV.Length;
		var plainBytes = decryptor.TransformFinalBlock(data, ivLength, data.Length - ivLength);

		return Encoding.UTF8.GetString(plainBytes);
	}
}
