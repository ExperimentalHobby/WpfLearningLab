using VirtualizedLogViewer.Models;

namespace VirtualizedLogViewer.Services;

/// <summary>
/// キーワード・ログレベルでログ行を絞り込む。
/// </summary>
public static class LogLineFilter
{
	/// <summary>絞り込みを行わないことを表すレベル指定。</summary>
	public const string AllLevels = "ALL";

	/// <summary>
	/// <paramref name="keyword"/>(メッセージの部分一致)と<paramref name="level"/>(完全一致、<see cref="AllLevels"/>は絞り込まない)で絞り込む。
	/// </summary>
	public static IReadOnlyList<LogLine> Filter(IReadOnlyList<LogLine> lines, string? keyword, string? level)
	{
		IEnumerable<LogLine> query = lines;

		if (!string.IsNullOrWhiteSpace(keyword))
		{
			query = query.Where(line => line.Message.Contains(keyword, StringComparison.OrdinalIgnoreCase));
		}

		if (!string.IsNullOrWhiteSpace(level) && !string.Equals(level, AllLevels, StringComparison.OrdinalIgnoreCase))
		{
			query = query.Where(line => string.Equals(line.Level, level, StringComparison.OrdinalIgnoreCase));
		}

		return query.ToList();
	}
}
