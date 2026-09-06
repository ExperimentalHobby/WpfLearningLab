using System.IO;
using System.Text;

namespace MiniCodeEditor.Services;

/// <summary>
/// 実の<see cref="File"/>操作を使う<see cref="IFileService"/>実装。
/// </summary>
public class FileService : IFileService
{
	// BOM無しUTF-8を既定のエンコーディングとして明示する。多様なエンコーディングの自動判別・
	// 選択機能(Shift-JIS等)はスコープが大きいため対象外とする。
	private static readonly UTF8Encoding Utf8WithoutBom = new(encoderShouldEmitUTF8Identifier: false);

	/// <inheritdoc/>
	public string ReadAllText(string filePath) => File.ReadAllText(filePath, Utf8WithoutBom);

	/// <inheritdoc/>
	public void WriteAllText(string filePath, string content) => File.WriteAllText(filePath, content, Utf8WithoutBom);
}
