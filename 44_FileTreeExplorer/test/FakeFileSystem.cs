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

    public void SetDirectories(string path, IReadOnlyList<string> directories) => _directories[path] = directories;

    public void SetFiles(string path, IReadOnlyList<FileEntry> files) => _files[path] = files;

    public void SetDirectoryException(string path, Exception exception) => _directoryExceptions[path] = exception;

    public void SetFileException(string path, Exception exception) => _fileExceptions[path] = exception;

    public IReadOnlyList<string> GetDirectories(string path)
    {
        if (_directoryExceptions.TryGetValue(path, out var ex))
        {
            throw ex;
        }

        return _directories.TryGetValue(path, out var dirs) ? dirs : Array.Empty<string>();
    }

    public IReadOnlyList<FileEntry> GetFiles(string path)
    {
        if (_fileExceptions.TryGetValue(path, out var ex))
        {
            throw ex;
        }

        return _files.TryGetValue(path, out var files) ? files : Array.Empty<FileEntry>();
    }
}
