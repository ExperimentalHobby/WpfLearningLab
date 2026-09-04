using ContactManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.Data;

/// <summary>
/// EF Core(SQLite)を使った<see cref="IContactRepository"/>の実装。
/// </summary>
public class EfContactRepository : IContactRepository
{
	private readonly ContactManagerDbContext _context;

	/// <summary>
	/// <see cref="EfContactRepository"/>を初期化する。
	/// </summary>
	public EfContactRepository(ContactManagerDbContext context)
	{
		_context = context;
	}

	/// <inheritdoc/>
	public IReadOnlyList<Contact> GetAll() => _context.Contacts.OrderBy(contact => contact.Name).ToList();

	/// <inheritdoc/>
	public void Add(Contact contact)
	{
		_context.Contacts.Add(contact);
		_context.SaveChanges();
	}

	/// <inheritdoc/>
	public void Update(Contact contact)
	{
		_context.Contacts.Update(contact);
		_context.SaveChanges();
	}

	/// <inheritdoc/>
	public void Delete(int contactId)
	{
		var entity = _context.Contacts.Find(contactId);
		if (entity is not null)
		{
			_context.Contacts.Remove(entity);
			_context.SaveChanges();
		}
	}
}
