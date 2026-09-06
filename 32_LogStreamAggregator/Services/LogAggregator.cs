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

	// Add()はバックグラウンドのConsumerスレッドから、各読み取りプロパティはUIスレッドから
	// 呼ばれる想定。現状はIUiDispatcher.Invokeが同期ブロッキング(Dispatcher.Invoke)のため
	// 実質的に直列化されているが、将来InvokeAsync等に変更された場合でも壊れないよう、
	// ロックで保護し読み取りは辞書のコピーを返す。
	private readonly Lock _lock = new();
	private readonly Dictionary<LogLevel, int> _countsByLevel;
	private readonly Dictionary<string, int> _keywordCounts;
	private int _totalCount;

	/// <summary>
	/// <see cref="LogAggregator"/>を初期化する。
	/// </summary>
	public LogAggregator()
	{
		_countsByLevel = Enum.GetValues<LogLevel>().ToDictionary(level => level, _ => 0);
		_keywordCounts = WatchedKeywords.ToDictionary(keyword => keyword, _ => 0);
	}

	/// <summary>集計した総件数。</summary>
	public int TotalCount
	{
		get
		{
			lock (_lock)
			{
				return _totalCount;
			}
		}
	}

	/// <summary>ログレベル別の件数(呼び出し時点のスナップショット)。</summary>
	public IReadOnlyDictionary<LogLevel, int> CountsByLevel
	{
		get
		{
			lock (_lock)
			{
				return new Dictionary<LogLevel, int>(_countsByLevel);
			}
		}
	}

	/// <summary>キーワード別の出現回数(呼び出し時点のスナップショット)。</summary>
	public IReadOnlyDictionary<string, int> KeywordCounts
	{
		get
		{
			lock (_lock)
			{
				return new Dictionary<string, int>(_keywordCounts);
			}
		}
	}

	/// <summary>
	/// 1件のログを集計に反映する。
	/// </summary>
	public void Add(LogEntry entry)
	{
		lock (_lock)
		{
			_totalCount++;
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
}
