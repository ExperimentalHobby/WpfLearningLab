using System.IO;
using VirtualizedLogViewer.Models;

namespace VirtualizedLogViewer.Services;

/// <summary>
/// ログファイルを読み込む。
/// </summary>
public static class LogFileLoader
{
	/// <summary>
	/// <paramref name="filePath"/>を1行ずつストリーミングで読み込み、<see cref="LogLine"/>のリストに変換する。
	/// <see cref="File.ReadLines(string)"/>を使うため、ファイル全体を一度に文字列として読み込まない。
	/// </summary>
	public static List<LogLine> Load(string filePath)
	{
		var result = new List<LogLine>();
		var lineNumber = 0;
		foreach (var rawLine in File.ReadLines(filePath))
		{
			lineNumber++;
			result.Add(LogLineFormatter.Parse(lineNumber, rawLine));
		}
		return result;
	}
}
