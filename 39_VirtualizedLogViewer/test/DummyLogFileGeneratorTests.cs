using VirtualizedLogViewer.Services;

namespace VirtualizedLogViewer.Tests;

/// <summary>
/// <see cref="DummyLogFileGenerator"/>のテスト。
/// </summary>
public class DummyLogFileGeneratorTests : IDisposable
{
	private readonly string _filePath;

	public DummyLogFileGeneratorTests()
	{
		_filePath = Path.Combine(Path.GetTempPath(), $"VirtualizedLogViewerTests_{Guid.NewGuid():N}.log");
	}

	public void Dispose()
	{
		if (File.Exists(_filePath))
		{
			File.Delete(_filePath);
		}
	}

	/// <summary>
	/// パス条件: 指定した行数のログがファイルに書き出されること。
	/// </summary>
	[Fact]
	public void GenerateToFile_指定した行数がファイルに書き出される()
	{
		var generator = new DummyLogFileGenerator(new Random(1));

		generator.GenerateToFile(_filePath, 100);

		Assert.Equal(100, File.ReadLines(_filePath).Count());
	}

	/// <summary>
	/// パス条件: 同一シードなら決定的に同じ内容を生成すること。
	/// </summary>
	[Fact]
	public void GenerateToFile_同一シードなら決定的に同じ内容を生成する()
	{
		var filePathB = _filePath + ".b";
		try
		{
			new DummyLogFileGenerator(new Random(42)).GenerateToFile(_filePath, 50);
			new DummyLogFileGenerator(new Random(42)).GenerateToFile(filePathB, 50);

			Assert.Equal(File.ReadAllText(_filePath), File.ReadAllText(filePathB));
		}
		finally
		{
			if (File.Exists(filePathB))
			{
				File.Delete(filePathB);
			}
		}
	}
}
