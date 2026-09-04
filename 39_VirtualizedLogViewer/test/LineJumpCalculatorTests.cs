using VirtualizedLogViewer.Models;
using VirtualizedLogViewer.Services;

namespace VirtualizedLogViewer.Tests;

/// <summary>
/// <see cref="LineJumpCalculator"/>のテスト。
/// </summary>
public class LineJumpCalculatorTests
{
	/// <summary>
	/// パス条件: 正の整数文字列は行番号として解析できること。
	/// </summary>
	[Fact]
	public void TryParseLineNumber_正の整数文字列は解析できる()
	{
		Assert.True(LineJumpCalculator.TryParseLineNumber("42", out var lineNumber));
		Assert.Equal(42, lineNumber);
	}

	/// <summary>
	/// パス条件: 0以下の数値・数値でない文字列は解析に失敗すること。
	/// </summary>
	[Fact]
	public void TryParseLineNumber_0以下や数値でない文字列は失敗する()
	{
		Assert.False(LineJumpCalculator.TryParseLineNumber("0", out _));
		Assert.False(LineJumpCalculator.TryParseLineNumber("-1", out _));
		Assert.False(LineJumpCalculator.TryParseLineNumber("abc", out _));
	}

	/// <summary>
	/// パス条件: 表示中のリストに指定した行番号が存在する場合、その位置(0始まり)を返すこと。
	/// </summary>
	[Fact]
	public void FindDisplayIndex_存在する行番号はその位置を返す()
	{
		List<LogLine> lines = [new(10, "INFO", "a"), new(20, "INFO", "b"), new(30, "INFO", "c")];

		var index = LineJumpCalculator.FindDisplayIndex(lines, 20);

		Assert.Equal(1, index);
	}

	/// <summary>
	/// パス条件: フィルタにより対象行が表示中のリストに存在しない場合、-1を返すこと。
	/// </summary>
	[Fact]
	public void FindDisplayIndex_存在しない行番号は負1を返す()
	{
		List<LogLine> lines = [new(10, "INFO", "a"), new(30, "INFO", "c")];

		var index = LineJumpCalculator.FindDisplayIndex(lines, 20);

		Assert.Equal(-1, index);
	}
}
