using System.IO;

namespace VirtualizedLogViewer.Services;

/// <summary>
/// 大量行のダミーログをファイルへ生成する疑似プロデューサ。
/// </summary>
public class DummyLogFileGenerator
{
	private static readonly string[] Levels = ["INFO", "WARN", "ERROR", "DEBUG"];

	private readonly Random _random;

	/// <summary>
	/// <see cref="DummyLogFileGenerator"/>を初期化する。
	/// </summary>
	/// <param name="random">乱数生成器。テスト時は決定的な結果を得るため固定シードを渡す。</param>
	public DummyLogFileGenerator(Random random)
	{
		_random = random;
	}

	/// <summary>
	/// 指定した行数のダミーログを<paramref name="filePath"/>へ書き出す。
	/// メモリに全件保持せずストリーミングで書き込むため、大量行でもメモリ使用量を抑えられる。
	/// </summary>
	public void GenerateToFile(string filePath, int lineCount)
	{
		using var writer = new StreamWriter(filePath, append: false);
		for (var i = 1; i <= lineCount; i++)
		{
			var level = Levels[_random.Next(Levels.Length)];
			writer.WriteLine(LogLineFormatter.Format(level, $"Sample log message number {i}"));
		}
	}
}
