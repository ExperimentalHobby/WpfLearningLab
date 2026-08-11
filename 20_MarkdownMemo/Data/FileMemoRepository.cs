using System.IO;
using MarkdownMemo.Models;

namespace MarkdownMemo.Data;

/// <summary>
/// メモ1件につき1個の<c>.md</c>ファイルとしてフォルダに保存するリポジトリ。
/// タイトルがファイル名(識別子)を兼ねる。
/// </summary>
public class FileMemoRepository : IMemoRepository
{
	private const string Extension = ".md";

	private readonly string _folderPath;

	/// <summary>
	/// リポジトリを初期化し、保存先フォルダが無ければ作成する。
	/// </summary>
	/// <param name="folderPath">メモファイルを保存するフォルダのパス。</param>
	public FileMemoRepository(string folderPath)
	{
		_folderPath = folderPath;
		Directory.CreateDirectory(_folderPath);
	}

	/// <inheritdoc/>
	public IReadOnlyList<MemoSummary> GetAll() =>
		Directory.GetFiles(_folderPath, $"*{Extension}")
			.Select(path => new MemoSummary(Path.GetFileNameWithoutExtension(path), File.GetLastWriteTime(path)))
			.OrderByDescending(memo => memo.LastModified)
			.ToList();

	/// <inheritdoc/>
	public string Load(string title) => File.ReadAllText(GetPath(title));

	/// <inheritdoc/>
	public void Save(string title, string content) => File.WriteAllText(GetPath(title), content);

	/// <inheritdoc/>
	public void Delete(string title)
	{
		var path = GetPath(title);
		if (File.Exists(path))
		{
			File.Delete(path);
		}
	}

	private string GetPath(string title) => Path.Combine(_folderPath, $"{title}{Extension}");
}
