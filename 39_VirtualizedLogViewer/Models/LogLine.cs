namespace VirtualizedLogViewer.Models;

/// <summary>
/// ログファイルの1行。
/// </summary>
/// <param name="LineNumber">行番号(1始まり)。</param>
/// <param name="Level">ログレベル文字列(INFO/WARN/ERROR等)。</param>
/// <param name="Message">メッセージ本文。</param>
public record LogLine(int LineNumber, string Level, string Message);
