using PasswordManager.Data;

namespace PasswordManager.Tests;

/// <summary>
/// <see cref="MainViewModel"/> のテスト用に使う<see cref="IMasterKeyStore"/>のフェイク実装。
/// </summary>
public class FakeMasterKeyStore : IMasterKeyStore
{
	private byte[]? _salt;
	private string? _verificationValue;

	public bool IsInitialized() => _salt is not null;

	public byte[] GetSalt() => _salt ?? throw new InvalidOperationException("マスターキーが初期設定されていません。");

	public string GetVerificationValue() => _verificationValue ?? throw new InvalidOperationException("マスターキーが初期設定されていません。");

	public void Initialize(byte[] salt, string verificationValue)
	{
		_salt = salt;
		_verificationValue = verificationValue;
	}
}
