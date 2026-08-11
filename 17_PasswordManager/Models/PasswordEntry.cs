namespace PasswordManager.Models;

/// <summary>
/// 1件のパスワードエントリを表すエンティティ。<see cref="EncryptedPassword"/> は常に暗号化された状態で保持する。
/// </summary>
public class PasswordEntry
{
	/// <summary>エントリID(SQLiteの自動採番)。新規作成時は0。</summary>
	public int Id { get; set; }

	/// <summary>サイト名。</summary>
	public required string Site { get; set; }

	/// <summary>ユーザー名。</summary>
	public required string Username { get; set; }

	/// <summary>暗号化されたパスワード(Base64)。</summary>
	public required string EncryptedPassword { get; set; }
}
