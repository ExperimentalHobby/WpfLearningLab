using System.IO;
using MiniCodeEditor.Services;

namespace MiniCodeEditor.Tests;

/// <summary>
/// <see cref="ViewModels.MainViewModel"/> のテスト用に、実際のファイルI/Oを行わない<see cref="IFileService"/>実装。
/// </summary>
public class FakeFileService : IFileService
{
	private readonly Dictionary<string, string> _files = [];

	/// <summary>最後に<see cref="WriteAllText"/>に渡されたパス。</summary>
	public string? LastWrittenPath { get; private set; }

	/// <summary>最後に<see cref="WriteAllText"/>に渡された内容。</summary>
	public string? LastWrittenContent { get; private set; }

	/// <summary>
	/// テストの前提として、指定パスに読み込み可能なファイルを用意する。
	/// </summary>
	public void SeedFile(string filePath, string content) => _files[filePath] = content;

	/// <inheritdoc/>
	public string ReadAllText(string filePath) => _files.TryGetValue(filePath, out var content)
		? content
		: throw new FileNotFoundException($"ファイルが見つかりません: {filePath}");

	/// <inheritdoc/>
	public void WriteAllText(string filePath, string content)
	{
		_files[filePath] = content;
		LastWrittenPath = filePath;
		LastWrittenContent = content;
	}
}
