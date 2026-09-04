using ContactManager.Models;

namespace ContactManager.Data;

/// <summary>
/// 連絡先の永続化を担うリポジトリの抽象。
/// </summary>
public interface IContactRepository
{
	/// <summary>登録済みの全連絡先を氏名順で取得する。</summary>
	IReadOnlyList<Contact> GetAll();

	/// <summary>連絡先を新規追加する。追加後、<paramref name="contact"/>の<see cref="Contact.Id"/>が更新される。</summary>
	void Add(Contact contact);

	/// <summary>連絡先を更新する。</summary>
	void Update(Contact contact);

	/// <summary>指定したIDの連絡先を削除する。</summary>
	void Delete(int contactId);
}
