using MarkdownMemo.Data;
using MarkdownMemo.Models;

namespace MarkdownMemo.Tests;

/// <summary>
/// <see cref="ViewModels.MainViewModel"/> のテスト用に使う<see cref="IMemoRepository"/>のフェイク実装。
/// メモリ上の辞書でCRUDを模倣する。
/// </summary>
public class FakeMemoRepository : IMemoRepository
{
	private readonly Dictionary<string, string> _memos = [];
	private readonly Dictionary<string, DateTime> _lastModified = [];
	private int _sequence;

	public IReadOnlyList<MemoSummary> GetAll() =>
		_memos.Keys
			.Select(title => new MemoSummary(title, _lastModified[title]))
			.OrderByDescending(memo => memo.LastModified)
			.ToList();

	public string Load(string title) => _memos[title];

	public void Save(string title, string content)
	{
		_memos[title] = content;
		// 実ファイルの最終更新日時と同様、Save順に新しい日時になるよう連番から生成する。
		_lastModified[title] = new DateTime(2026, 1, 1).AddSeconds(_sequence++);
	}

	public void Delete(string title)
	{
		_memos.Remove(title);
		_lastModified.Remove(title);
	}
}
