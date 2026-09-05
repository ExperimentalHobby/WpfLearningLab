using ClaudeChatClient.Services;

namespace ClaudeChatClient.Tests;

public class AesApiKeyCryptoServiceTests
{
	/// <summary>
	/// パス条件: 暗号化した文字列を同じ鍵で復号すると元の平文に戻ること。
	/// </summary>
	[Fact]
	public void EncryptThenDecrypt_ReturnsOriginalPlainText()
	{
		var service = new AesApiKeyCryptoService();
		var salt = service.GenerateSalt();
		var key = service.DeriveKey("master-password", salt);

		var cipherText = service.Encrypt("sk-ant-api-key-12345", key);
		var plainText = service.Decrypt(cipherText, key);

		Assert.Equal("sk-ant-api-key-12345", plainText);
	}

	/// <summary>
	/// パス条件: 同一パスワード+ソルトからは常に同じ鍵が導出されること。
	/// </summary>
	[Fact]
	public void DeriveKey_SamePasswordAndSalt_ProducesSameKey()
	{
		var service = new AesApiKeyCryptoService();
		var salt = service.GenerateSalt();

		var key1 = service.DeriveKey("master-password", salt);
		var key2 = service.DeriveKey("master-password", salt);

		Assert.Equal(key1, key2);
	}

	/// <summary>
	/// パス条件: ソルトが異なれば導出される鍵も変わること。
	/// </summary>
	[Fact]
	public void DeriveKey_DifferentSalt_ProducesDifferentKey()
	{
		var service = new AesApiKeyCryptoService();
		var salt1 = service.GenerateSalt();
		var salt2 = service.GenerateSalt();

		var key1 = service.DeriveKey("master-password", salt1);
		var key2 = service.DeriveKey("master-password", salt2);

		Assert.NotEqual(key1, key2);
	}

	/// <summary>
	/// パス条件: 誤った鍵で復号すると例外がスローされること。
	/// </summary>
	[Fact]
	public void Decrypt_WithWrongKey_Throws()
	{
		var service = new AesApiKeyCryptoService();
		var salt = service.GenerateSalt();
		var correctKey = service.DeriveKey("master-password", salt);
		var wrongKey = service.DeriveKey("wrong-password", salt);

		var cipherText = service.Encrypt("sk-ant-api-key-12345", correctKey);

		Assert.ThrowsAny<Exception>(() => service.Decrypt(cipherText, wrongKey));
	}
}
