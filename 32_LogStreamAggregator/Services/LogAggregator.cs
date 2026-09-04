using LogStreamAggregator.Models;

namespace LogStreamAggregator.Services;

/// <summary>
/// ログレベル別件数・キーワード出現回数を集計する純粋なロジック。
/// チャネル等の非同期処理から独立しているため、決定的かつ高速にテストできる。
/// </summary>
public class LogAggregator
{
	/// <summary>集計対象として監視するキーワード。</summary>
	public static readonly IReadOnlyList<string> WatchedKeywords = ["Exception", "Timeout", "Retry"];

	private readonly Dictionary<LogLevel, int> _countsByLevel;
	private readonly Dictionary<string, int> _keywordCounts;

	/// <summary>
	/// <see cref="LogAggregator"/>を初期化する。
	/// </summary>
	public LogAggregator()
	{
		_countsByLevel = Enum.GetValues<LogLevel>().ToDictionary(level => level, _ => 0);
		_keywordCounts = WatchedKeywords.ToDictionary(keyword => keyword, _ => 0);
	}

	/// <summary>集計した総件数。</summary>
	public int TotalCount { get; private set; }

	/// <summary>ログレベル別の件数。</summary>
	public IReadOnlyDictionary<LogLevel, int> CountsByLevel => _countsByLevel;

	/// <summary>キーワード別の出現回数。</summary>
	public IReadOnlyDictionary<string, int> KeywordCounts => _keywordCounts;

	/// <summary>
	/// 1件のログを集計に反映する。
	/// </summary>
	public void Add(LogEntry entry)
	{
		TotalCount++;
		_countsByLevel[entry.Level]++;
		foreach (var keyword in WatchedKeywords)
		{
			if (entry.Message.Contains(keyword, StringComparison.OrdinalIgnoreCase))
			{
				_keywordCounts[keyword]++;
			}
		}
	}
}
