using VirtualizedLogViewer.Models;

namespace VirtualizedLogViewer.Services;

/// <summary>
/// ログ行のテキスト表現(<c>"LEVEL メッセージ"</c>形式)との相互変換を行う。
/// </summary>
public static class LogLineFormatter
{
	/// <summary>
	/// レベルとメッセージをファイルに書き込む1行のテキストに整形する。
	/// </summary>
	public static string Format(string level, string message) => $"{level} {message}";

	/// <summary>
	/// テキスト1行を<see cref="LogLine"/>に変換する。先頭の空白区切りの単語をレベルとして扱う。
	/// </summary>
	public static LogLine Parse(int lineNumber, string rawLine)
	{
		var spaceIndex = rawLine.IndexOf(' ');
		if (spaceIndex < 0)
		{
			return new LogLine(lineNumber, "INFO", rawLine);
		}
		var level = rawLine[..spaceIndex];
		var message = rawLine[(spaceIndex + 1)..];
		return new LogLine(lineNumber, level, message);
	}
}
