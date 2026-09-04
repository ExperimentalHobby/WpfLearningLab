using SystemTrayUtility.Services;

namespace SystemTrayUtility.Tests;

/// <summary>
/// <see cref="ReminderIntervalParser"/>のテスト。
/// </summary>
public class ReminderIntervalParserTests
{
	/// <summary>
	/// パス条件: 正の整数を指定すると、その分数の間隔に変換されること。
	/// </summary>
	[Fact]
	public void TryParseMinutes_正の整数はその分数の間隔になる()
	{
		var result = ReminderIntervalParser.TryParseMinutes("15", out var interval);

		Assert.True(result);
		Assert.Equal(TimeSpan.FromMinutes(15), interval);
	}

	/// <summary>
	/// パス条件: 0以下の数値は失敗すること。
	/// </summary>
	[Fact]
	public void TryParseMinutes_0以下は失敗する()
	{
		Assert.False(ReminderIntervalParser.TryParseMinutes("0", out _));
		Assert.False(ReminderIntervalParser.TryParseMinutes("-5", out _));
	}

	/// <summary>
	/// パス条件: 数値でない文字列は失敗すること。
	/// </summary>
	[Fact]
	public void TryParseMinutes_数値でない場合は失敗する()
	{
		Assert.False(ReminderIntervalParser.TryParseMinutes("abc", out _));
	}
}
