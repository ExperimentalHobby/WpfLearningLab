namespace FileTreeExplorer.Tests;

/// <summary>
/// <see cref="IFileSystem"/> のテスト用Fake実装。
/// パスごとに返すサブフォルダ/ファイル、または送出する例外を設定できる。
/// </summary>
public class FakeFileSystem : IFileSystem
{
	private readonly Dictionary<string, IReadOnlyList<string>> _directories = new();
	private readonly Dictionary<string, IReadOnlyList<FileEntry>> _files = new();
	private readonly Dictionary<string, Exception> _directoryExceptions = new();
	private readonly Dictionary<string, Exception> _fileExceptions = new();

	/// <summary>指定パスに対して<see cref="GetDirectories"/>が返すサブフォルダ一覧を設定する(テスト用)。</summary>
	public void SetDirectories(string path, IReadOnlyList<string> directories) => _directories[path] = directories;

	/// <summary>指定パスに対して<see cref="GetFiles"/>が返すファイル一覧を設定する(テスト用)。</summary>
	public void SetFiles(string path, IReadOnlyList<FileEntry> files) => _files[path] = files;

	/// <summary>指定パスで<see cref="GetDirectories"/>呼び出し時にスローする例外を設定する(テスト用)。</summary>
	public void SetDirectoryException(string path, Exception exception) => _directoryExceptions[path] = exception;

	/// <summary>指定パスに設定した<see cref="SetDirectoryException"/>の例外設定を解除する(テスト用)。</summary>
	public void ClearDirectoryException(string path) => _directoryExceptions.Remove(path);

	/// <summary>指定パスで<see cref="GetFiles"/>呼び出し時にスローする例外を設定する(テスト用)。</summary>
	public void SetFileException(string path, Exception exception) => _fileExceptions[path] = exception;

	/// <inheritdoc/>
	public IReadOnlyList<string> GetDirectories(string path)
	{
		if (_directoryExceptions.TryGetValue(path, out var ex))
		{
			throw ex;
		}

		return _directories.TryGetValue(path, out var dirs) ? dirs : Array.Empty<string>();
	}

	/// <inheritdoc/>
	public IReadOnlyList<FileEntry> GetFiles(string path)
	{
		if (_fileExceptions.TryGetValue(path, out var ex))
		{
			throw ex;
		}

		return _files.TryGetValue(path, out var files) ? files : Array.Empty<FileEntry>();
	}
}
