namespace LogStreamAggregator.Models;

/// <summary>
/// ログの重要度レベル。
/// </summary>
public enum LogLevel
{
	/// <summary>デバッグ情報。</summary>
	Debug,

	/// <summary>通常の情報。</summary>
	Info,

	/// <summary>警告。</summary>
	Warning,

	/// <summary>エラー。</summary>
	Error,
}
