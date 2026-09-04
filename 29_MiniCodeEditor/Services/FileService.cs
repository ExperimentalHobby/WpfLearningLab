using System.IO;

namespace MiniCodeEditor.Services;

/// <summary>
/// 実の<see cref="File"/>操作を使う<see cref="IFileService"/>実装。
/// </summary>
public class FileService : IFileService
{
	/// <inheritdoc/>
	public string ReadAllText(string filePath) => File.ReadAllText(filePath);

	/// <inheritdoc/>
	public void WriteAllText(string filePath, string content) => File.WriteAllText(filePath, content);
}
