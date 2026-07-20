namespace MiniDictionary;

/// <summary>
/// 単語→意味の対応を保持し、部分一致検索と意味の取得を行うエンジン。
/// </summary>
public class DictionaryEngine
{
	private static readonly Dictionary<string, string> SampleEntries = new()
	{
		["apple"] = "りんご",
		["banana"] = "バナナ",
		["cat"] = "猫",
		["dog"] = "犬",
		["book"] = "本",
		["computer"] = "コンピュータ",
		["water"] = "水",
		["school"] = "学校",
		["friend"] = "友達",
		["music"] = "音楽",
	};

	private readonly Dictionary<string, string> _entries;

	/// <summary>
	/// サンプルの辞書データでエンジンを初期化する。
	/// </summary>
	public DictionaryEngine() : this(SampleEntries)
	{
	}

	/// <summary>
	/// 辞書データを指定してエンジンを初期化する。
	/// </summary>
	/// <param name="entries">単語をキー、意味を値とする辞書データ。</param>
	public DictionaryEngine(IDictionary<string, string> entries)
	{
		_entries = new Dictionary<string, string>(entries);
	}

	/// <summary>
	/// クエリに部分一致(大文字小文字を区別しない)する単語を、辞書順にソートして返す。
	/// クエリが空の場合は全単語を返す。
	/// </summary>
	/// <param name="query">検索文字列。</param>
	public IReadOnlyList<string> Search(string query)
	{
		return _entries.Keys
			.Where(word => query.Length == 0 || word.Contains(query, StringComparison.OrdinalIgnoreCase))
			.OrderBy(word => word, StringComparer.Ordinal)
			.ToList();
	}

	/// <summary>
	/// 指定した単語の意味を取得する。
	/// </summary>
	/// <param name="word">検索する単語(完全一致)。</param>
	/// <returns>意味の文字列。存在しない場合は null。</returns>
	public string? GetMeaning(string word)
	{
		return _entries.TryGetValue(word, out var meaning) ? meaning : null;
	}
}
