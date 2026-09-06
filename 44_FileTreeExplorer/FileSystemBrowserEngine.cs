using System.IO;

namespace FileTreeExplorer;

/// <summary>
/// フォルダ・ファイルの一覧取得を、アクセス拒否等の例外を吸収しつつ行うロジック。
/// </summary>
public class FileSystemBrowserEngine
{
	private readonly IFileSystem _fileSystem;

	public FileSystemBrowserEngine(IFileSystem fileSystem)
	{
		_fileSystem = fileSystem;
	}

	/// <summary>
	/// 指定フォルダ直下のサブフォルダ一覧の取得を試みる。
	/// アクセス拒否等の例外が発生した場合は false を返し、空一覧とエラーメッセージを設定する。
	/// </summary>
	public bool TryGetSubFolders(string path, out IReadOnlyList<string> folders, out string? errorMessage)
	{
		try
		{
			folders = _fileSystem.GetDirectories(path);
			errorMessage = null;
			return true;
		}
		catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
		{
			folders = Array.Empty<string>();
			errorMessage = "アクセスが拒否されました。";
			return false;
		}
	}

	/// <summary>
	/// 指定フォルダ直下のファイル一覧の取得を試みる。
	/// アクセス拒否等の例外が発生した場合は false を返し、空一覧とエラーメッセージを設定する。
	/// </summary>
	public bool TryGetFiles(string path, out IReadOnlyList<FileEntry> files, out string? errorMessage)
	{
		try
		{
			files = _fileSystem.GetFiles(path);
			errorMessage = null;
			return true;
		}
		catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
		{
			files = Array.Empty<FileEntry>();
			errorMessage = "アクセスが拒否されました。";
			return false;
		}
	}
}
