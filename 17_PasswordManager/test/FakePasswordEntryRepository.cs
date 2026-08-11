using PasswordManager.Data;
using PasswordManager.Models;

namespace PasswordManager.Tests;

/// <summary>
/// <see cref="MainViewModel"/> のテスト用に使う<see cref="IPasswordEntryRepository"/>のフェイク実装。
/// メモリ上のリストでCRUDを模倣する。
/// </summary>
public class FakePasswordEntryRepository : IPasswordEntryRepository
{
	private readonly List<PasswordEntry> _entries = [];
	private int _nextId = 1;

	public IReadOnlyList<PasswordEntry> GetAll() => _entries.ToList();

	public void Add(PasswordEntry entry)
	{
		entry.Id = _nextId++;
		_entries.Add(entry);
	}

	public void Update(PasswordEntry entry)
	{
		var index = _entries.FindIndex(e => e.Id == entry.Id);
		if (index >= 0)
		{
			_entries[index] = entry;
		}
	}

	public void Delete(int id) => _entries.RemoveAll(e => e.Id == id);
}
