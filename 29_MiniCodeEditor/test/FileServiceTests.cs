using MiniCodeEditor.Services;

namespace MiniCodeEditor.Tests;

/// <summary>
/// <see cref="FileService"/> の単体テスト。実の一時フォルダに対して検証する。
/// </summary>
public class FileServiceTests : IDisposable
{
	private readonly string _tempDirectory;

	public FileServiceTests()
	{
		_tempDirectory = Path.Combine(Path.GetTempPath(), $"MiniCodeEditorTests_{Guid.NewGuid():N}");
		Directory.CreateDirectory(_tempDirectory);
	}

	public void Dispose()
	{
		if (Directory.Exists(_tempDirectory))
		{
			Directory.Delete(_tempDirectory, recursive: true);
		}
	}

	/// <summary>
	/// パス条件: WriteAllTextで書き込んだ内容を、ReadAllTextで読み込めること
	/// </summary>
	[Fact]
	public void WriteAllText_書き込んだ内容をReadAllTextで読み込める()
	{
		var filePath = Path.Combine(_tempDirectory, "sample.cs");
		var service = new FileService();

		service.WriteAllText(filePath, "class C { }");
		var content = service.ReadAllText(filePath);

		Assert.Equal("class C { }", content);
	}

	/// <summary>
	/// パス条件: 既存ファイルにWriteAllTextすると、内容が上書きされること
	/// </summary>
	[Fact]
	public void WriteAllText_既存ファイルへの書き込みは上書きされる()
	{
		var filePath = Path.Combine(_tempDirectory, "sample.cs");
		var service = new FileService();
		service.WriteAllText(filePath, "old content");

		service.WriteAllText(filePath, "new content");

		Assert.Equal("new content", service.ReadAllText(filePath));
	}

	/// <summary>
	/// パス条件: 存在しないファイルをReadAllTextすると、FileNotFoundExceptionが投げられること
	/// </summary>
	[Fact]
	public void ReadAllText_存在しないファイルはFileNotFoundExceptionを投げる()
	{
		var filePath = Path.Combine(_tempDirectory, "not-exist.cs");
		var service = new FileService();

		Assert.Throws<FileNotFoundException>(() => service.ReadAllText(filePath));
	}
}
