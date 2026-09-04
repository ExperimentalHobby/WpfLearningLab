using LogStreamAggregator.Models;
using LogStreamAggregator.Services;

namespace LogStreamAggregator.Tests;

/// <summary>
/// <see cref="LogAggregator"/>のテスト。チャネルを介さない純粋な集計ロジックを決定的に検証する。
/// </summary>
public class LogAggregatorTests
{
	/// <summary>
	/// パス条件: ログを追加すると、レベル別件数が正しく集計されること。
	/// </summary>
	[Fact]
	public void Add_ログレベル別件数が正しく集計される()
	{
		var sut = new LogAggregator();

		sut.Add(new LogEntry(DateTime.Now, LogLevel.Info, "started"));
		sut.Add(new LogEntry(DateTime.Now, LogLevel.Error, "failed"));
		sut.Add(new LogEntry(DateTime.Now, LogLevel.Error, "failed again"));

		Assert.Equal(3, sut.TotalCount);
		Assert.Equal(1, sut.CountsByLevel[LogLevel.Info]);
		Assert.Equal(2, sut.CountsByLevel[LogLevel.Error]);
		Assert.Equal(0, sut.CountsByLevel[LogLevel.Debug]);
	}

	/// <summary>
	/// パス条件: メッセージに監視対象キーワードが含まれる場合、キーワード別出現回数が加算されること。
	/// </summary>
	[Fact]
	public void Add_監視対象キーワードを含む場合キーワード出現回数が加算される()
	{
		var sut = new LogAggregator();

		sut.Add(new LogEntry(DateTime.Now, LogLevel.Error, "Unhandled Exception occurred"));
		sut.Add(new LogEntry(DateTime.Now, LogLevel.Warning, "Connection Timeout"));
		sut.Add(new LogEntry(DateTime.Now, LogLevel.Info, "Retry attempt 1"));
		sut.Add(new LogEntry(DateTime.Now, LogLevel.Info, "no keyword here"));

		Assert.Equal(1, sut.KeywordCounts["Exception"]);
		Assert.Equal(1, sut.KeywordCounts["Timeout"]);
		Assert.Equal(1, sut.KeywordCounts["Retry"]);
	}

	/// <summary>
	/// パス条件: 大文字小文字が異なっていてもキーワードとして一致すること。
	/// </summary>
	[Fact]
	public void Add_キーワードの大文字小文字を区別しない()
	{
		var sut = new LogAggregator();

		sut.Add(new LogEntry(DateTime.Now, LogLevel.Error, "unhandled exception in worker"));

		Assert.Equal(1, sut.KeywordCounts["Exception"]);
	}
}
