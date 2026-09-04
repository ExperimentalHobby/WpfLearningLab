namespace MiniCodeEditor.Services;

/// <summary>
/// ファイルの読み書きを行う処理の抽象。
/// </summary>
public interface IFileService
{
	/// <summary>
	/// 指定したパスのファイル内容をすべて読み込む。
	/// </summary>
	string ReadAllText(string filePath);

	/// <summary>
	/// 指定したパスにテキストを書き込む(既存ファイルは上書き)。
	/// </summary>
	void WriteAllText(string filePath, string content);
}
