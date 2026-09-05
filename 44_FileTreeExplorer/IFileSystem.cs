using System.IO;

namespace FileTreeExplorer;

/// <summary>
/// フォルダ内のファイル1件を表す。
/// </summary>
/// <param name="Name">ファイル名。</param>
/// <param name="SizeBytes">ファイルサイズ(バイト)。</param>
/// <param name="LastWriteTime">最終更新日時。</param>
public record FileEntry(string Name, long SizeBytes, DateTime LastWriteTime);

/// <summary>
/// ファイルシステムへのアクセスを抽象化するインターフェース。
/// テスト時はFakeに差し替え、実運用では <see cref="RealFileSystem"/> を使用する。
/// </summary>
public interface IFileSystem
{
    /// <summary>指定フォルダ直下のサブフォルダのフルパス一覧を取得する。</summary>
    IReadOnlyList<string> GetDirectories(string path);

    /// <summary>指定フォルダ直下のファイル一覧を取得する。</summary>
    IReadOnlyList<FileEntry> GetFiles(string path);
}

/// <summary>
/// <see cref="System.IO"/> を用いた <see cref="IFileSystem"/> の実装。
/// </summary>
public class RealFileSystem : IFileSystem
{
    /// <inheritdoc />
    public IReadOnlyList<string> GetDirectories(string path)
    {
        return Directory.GetDirectories(path);
    }

    /// <inheritdoc />
    public IReadOnlyList<FileEntry> GetFiles(string path)
    {
        return Directory.GetFiles(path)
            .Select(f =>
            {
                var info = new FileInfo(f);
                return new FileEntry(info.Name, info.Length, info.LastWriteTime);
            })
            .ToList();
    }
}
