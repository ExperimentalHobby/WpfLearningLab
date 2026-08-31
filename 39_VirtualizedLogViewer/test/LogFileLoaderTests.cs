using VirtualizedLogViewer.Services;

namespace VirtualizedLogViewer.Tests;

/// <summary>
/// <see cref="LogFileLoader"/>のテスト。実際の一時ファイルを読み込んで検証する。
/// </summary>
public class LogFileLoaderTests : IDisposable
{
	private readonly string _filePath;

	public LogFileLoaderTests()
	{
		_filePath = Path.Combine(Path.GetTempPath(), $"VirtualizedLogViewerLoadTests_{Guid.NewGuid():N}.log");
	}

	public void Dispose()
	{
		if (File.Exists(_filePath))
		{
			File.Delete(_filePath);
		}
	}

	/// <summary>
	/// パス条件: 書き込んだ内容が行番号・レベル・メッセージ通りに読み込まれること。
	/// </summary>
	[Fact]
	public void Load_書き込んだ内容が行番号レベルメッセージ通りに読み込まれる()
	{
		File.WriteAllLines(_filePath, ["INFO first line", "ERROR something failed", "WARN careful"]);

		var lines = LogFileLoader.Load(_filePath);

		Assert.Equal(3, lines.Count);
		Assert.Equal(1, lines[0].LineNumber);
		Assert.Equal("INFO", lines[0].Level);
		Assert.Equal("first line", lines[0].Message);
		Assert.Equal("ERROR", lines[1].Level);
		Assert.Equal(3, lines[2].LineNumber);
	}
}
