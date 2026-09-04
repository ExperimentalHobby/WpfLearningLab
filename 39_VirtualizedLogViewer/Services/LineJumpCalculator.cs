using VirtualizedLogViewer.Models;

namespace VirtualizedLogViewer.Services;

/// <summary>
/// 行番号指定によるジャンプ先インデックスの計算・検証を行う。
/// フィルタ適用中は表示中のリストと元のログ行番号が一致しないため、入力を単純な位置(インデックス)としてではなく、
/// 表示中のリストの中から一致する<see cref="LogLine.LineNumber"/>を探す方式にしている。
/// </summary>
public static class LineJumpCalculator
{
	/// <summary>
	/// 入力文字列を1始まりの正の行番号として解析する。
	/// </summary>
	public static bool TryParseLineNumber(string input, out int lineNumber)
		=> int.TryParse(input, out lineNumber) && lineNumber > 0;

	/// <summary>
	/// 表示中のリストから、指定した行番号を持つ要素の位置(0始まり)を探す。見つからない場合は-1を返す。
	/// </summary>
	public static int FindDisplayIndex(IReadOnlyList<LogLine> displayedLines, int lineNumber)
	{
		for (var i = 0; i < displayedLines.Count; i++)
		{
			if (displayedLines[i].LineNumber == lineNumber)
			{
				return i;
			}
		}
		return -1;
	}
}
