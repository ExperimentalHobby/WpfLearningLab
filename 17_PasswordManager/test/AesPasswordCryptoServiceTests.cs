using System.Security.Cryptography;
using PasswordManager.Services;

namespace PasswordManager.Tests;

/// <summary>
/// <see cref="AesPasswordCryptoService"/> の単体テスト。
/// </summary>
public class AesPasswordCryptoServiceTests
{
	/// <summary>
	/// パス条件: 暗号化した文字列を同じ鍵で復号すると元の平文に戻ること
	/// </summary>
	[Fact]
	public void Encrypt_同じ鍵で復号すると元の平文に戻る()
	{
		var service = new AesPasswordCryptoService();
		var salt = service.GenerateSalt();
		var key = service.DeriveKey("master-password", salt);

		var cipherText = service.Encrypt("P@ssw0rd!", key);
		var decrypted = service.Decrypt(cipherText, key);

		Assert.Equal("P@ssw0rd!", decrypted);
	}

	/// <summary>
	/// パス条件: 同じマスターパスワードと同じソルトから鍵を導出すると常に同じ鍵になること
	/// </summary>
	[Fact]
	public void DeriveKey_同じパスワードと同じソルトなら同じ鍵になる()
	{
		var service = new AesPasswordCryptoService();
		var salt = service.GenerateSalt();

		var key1 = service.DeriveKey("master-password", salt);
		var key2 = service.DeriveKey("master-password", salt);

		Assert.Equal(key1, key2);
	}

	/// <summary>
	/// パス条件: 同じマスターパスワードでもソルトが異なれば導出される鍵も変わること
	/// </summary>
	[Fact]
	public void DeriveKey_ソルトが異なれば鍵も変わる()
	{
		var service = new AesPasswordCryptoService();
		var salt1 = service.GenerateSalt();
		var salt2 = service.GenerateSalt();

		var key1 = service.DeriveKey("master-password", salt1);
		var key2 = service.DeriveKey("master-password", salt2);

		Assert.NotEqual(key1, key2);
	}

	/// <summary>
	/// パス条件: 暗号化時と異なる鍵で復号すると例外がスローされること(マスターパスワード誤り検知に利用)
	/// </summary>
	[Fact]
	public void Decrypt_異なる鍵で復号すると例外がスローされる()
	{
		var service = new AesPasswordCryptoService();
		var correctKey = service.DeriveKey("correct-password", service.GenerateSalt());
		var wrongKey = service.DeriveKey("wrong-password", service.GenerateSalt());
		var cipherText = service.Encrypt("P@ssw0rd!", correctKey);

		Assert.ThrowsAny<CryptographicException>(() => service.Decrypt(cipherText, wrongKey));
	}

	/// <summary>
	/// パス条件: 異なる鍵の組み合わせで何度試行しても、必ず例外がスローされること
	/// (認証なしのAES-CBCだと、パディングが偶然妥当な形になり約1/256の確率で例外がすり抜けていた。
	/// 認証付き暗号への変更によりこれが確率的ではなく常に検知されることを確認する回帰テスト)
	/// </summary>
	[Fact]
	public void Decrypt_異なる鍵での複数回試行が常に例外になる()
	{
		var service = new AesPasswordCryptoService();

		for (var i = 0; i < 50; i++)
		{
			var correctKey = service.DeriveKey($"correct-password-{i}", service.GenerateSalt());
			var wrongKey = service.DeriveKey($"wrong-password-{i}", service.GenerateSalt());
			var cipherText = service.Encrypt("P@ssw0rd!", correctKey);

			Assert.ThrowsAny<CryptographicException>(() => service.Decrypt(cipherText, wrongKey));
		}
	}
}
