using ContactManager.Data;
using ContactManager.Models;

namespace ContactManager.Tests;

/// <summary>
/// <see cref="MainViewModel"/>のテストで実DBを使わずに済むようにする<see cref="IContactRepository"/>のフェイク。
/// </summary>
internal class FakeContactRepository : IContactRepository
{
	private readonly List<Contact> _contacts = [];
	private int _nextId = 1;

	public IReadOnlyList<Contact> GetAll() => _contacts.OrderBy(c => c.Name).ToList();

	public void Add(Contact contact)
	{
		contact.Id = _nextId++;
		_contacts.Add(contact);
	}

	public void Update(Contact contact)
	{
		// EFのUpdateと異なり、フェイクでは参照が同じインスタンスのため実質何もする必要はないが、
		// 呼び出し回数を確認できるようにカウントしておく。
		UpdateCallCount++;
	}

	public void Delete(int contactId)
	{
		_contacts.RemoveAll(c => c.Id == contactId);
	}

	public int UpdateCallCount { get; private set; }
}
