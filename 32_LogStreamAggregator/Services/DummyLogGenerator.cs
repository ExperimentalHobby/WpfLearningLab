using LogStreamAggregator.Models;

namespace LogStreamAggregator.Services;

/// <summary>
/// ダミーのログ行を生成する疑似プロデューサ。
/// </summary>
public class DummyLogGenerator
{
	private static readonly LogLevel[] Levels = Enum.GetValues<LogLevel>();

	private static readonly string[] MessageTemplates =
	[
		"Request completed successfully",
		"Connection Timeout occurred while calling external API",
		"Unhandled Exception in worker thread",
		"Retry attempt {0} for operation",
		"User logged in",
		"Cache miss for key {0}",
	];

	private readonly Random _random;

	/// <summary>
	/// <see cref="DummyLogGenerator"/>を初期化する。
	/// </summary>
	/// <param name="random">乱数生成器。テスト時は決定的な結果を得るため固定シードを渡す。</param>
	public DummyLogGenerator(Random random)
	{
		_random = random;
	}

	/// <summary>
	/// ランダムなログ行を1件生成する。
	/// </summary>
	public LogEntry Generate()
	{
		var level = Levels[_random.Next(Levels.Length)];
		var template = MessageTemplates[_random.Next(MessageTemplates.Length)];
		var message = string.Format(template, _random.Next(1, 100));
		return new LogEntry(DateTime.Now, level, message);
	}
}
