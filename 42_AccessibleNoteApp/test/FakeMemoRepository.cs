using AccessibleNoteApp.Models;
using AccessibleNoteApp.Services;

namespace AccessibleNoteApp.Tests;

/// <summary>
/// <see cref="IMemoRepository"/> のテスト用フェイク。実ファイルI/Oは行わずメモリ上で管理する。
/// </summary>
public class FakeMemoRepository : IMemoRepository
{
	private readonly Dictionary<string, Memo> _memos = [];

	/// <inheritdoc/>
	public IReadOnlyList<Memo> LoadAll() => _memos.Values.OrderByDescending(m => m.UpdatedAt).ToList();

	/// <inheritdoc/>
	public void Save(Memo memo) => _memos[memo.Id] = memo;

	/// <inheritdoc/>
	public void Delete(string id) => _memos.Remove(id);
}
