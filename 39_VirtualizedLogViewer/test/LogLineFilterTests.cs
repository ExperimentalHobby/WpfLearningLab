using VirtualizedLogViewer.Models;
using VirtualizedLogViewer.Services;

namespace VirtualizedLogViewer.Tests;

/// <summary>
/// <see cref="LogLineFilter"/>のテスト。
/// </summary>
public class LogLineFilterTests
{
	private static List<LogLine> CreateSampleLines() =>
	[
		new(1, "INFO", "server started"),
		new(2, "ERROR", "connection timeout"),
		new(3, "WARN", "disk space low"),
		new(4, "ERROR", "timeout while saving"),
	];

	/// <summary>
	/// パス条件: キーワードを指定すると、メッセージに含まれる行のみに絞り込まれること。
	/// </summary>
	[Fact]
	public void Filter_キーワード指定でメッセージに含まれる行のみ返す()
	{
		var result = LogLineFilter.Filter(CreateSampleLines(), "timeout", null);

		Assert.Equal(2, result.Count);
	}

	/// <summary>
	/// パス条件: レベルを指定すると、そのレベルの行のみに絞り込まれること。
	/// </summary>
	[Fact]
	public void Filter_レベル指定でそのレベルの行のみ返す()
	{
		var result = LogLineFilter.Filter(CreateSampleLines(), null, "ERROR");

		Assert.Equal(2, result.Count);
		Assert.All(result, line => Assert.Equal("ERROR", line.Level));
	}

	/// <summary>
	/// パス条件: キーワード・レベル両方を指定すると、両方を満たす行のみに絞り込まれること。
	/// </summary>
	[Fact]
	public void Filter_キーワードとレベル両方指定で両方満たす行のみ返す()
	{
		var result = LogLineFilter.Filter(CreateSampleLines(), "timeout", "ERROR");

		Assert.Equal(2, result.Count);
	}

	/// <summary>
	/// パス条件: ALLを指定した場合、レベルによる絞り込みは行われないこと。
	/// </summary>
	[Fact]
	public void Filter_ALL指定の場合レベルで絞り込まない()
	{
		var result = LogLineFilter.Filter(CreateSampleLines(), null, LogLineFilter.AllLevels);

		Assert.Equal(4, result.Count);
	}
}
