namespace LogStreamAggregator.Models;

/// <summary>
/// 1件のログ行。
/// </summary>
/// <param name="Timestamp">記録時刻。</param>
/// <param name="Level">ログレベル。</param>
/// <param name="Message">メッセージ本文。</param>
public record LogEntry(DateTime Timestamp, LogLevel Level, string Message);
