using PasswordManager.Models;

namespace PasswordManager.Data;

/// <summary>
/// パスワードエントリの永続化を担うリポジトリの抽象。
/// </summary>
public interface IPasswordEntryRepository
{
	/// <summary>
	/// 登録済みの全エントリを取得する。
	/// </summary>
	IReadOnlyList<PasswordEntry> GetAll();

	/// <summary>
	/// エントリを新規追加する。追加後、<paramref name="entry"/> の <see cref="PasswordEntry.Id"/> が更新される。
	/// </summary>
	void Add(PasswordEntry entry);

	/// <summary>
	/// 既存のエントリの内容を更新する。
	/// </summary>
	void Update(PasswordEntry entry);

	/// <summary>
	/// 指定したIDのエントリを削除する。
	/// </summary>
	void Delete(int id);
}
