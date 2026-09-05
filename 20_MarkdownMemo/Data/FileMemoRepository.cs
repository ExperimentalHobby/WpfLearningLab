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

	/// <summary>
	/// タイトルからファイルパスを組み立てる。タイトルはファイル名として使われるため、
	/// パス区切り文字(<c>/</c> <c>\</c>)や<c>:</c>等のファイル名として不正な文字を検証する。
	/// これによりタイトルは単一のパスセグメントに限定され、<c>../</c>による保存先フォルダ外への
	/// 書き込み(パストラバーサル)自体が成立しなくなる。
	/// </summary>
	private string GetPath(string title)
	{
		if (!IsValidTitle(title))
		{
			throw new ArgumentException($"タイトルに使用できない文字が含まれています: {title}", nameof(title));
		}

		return Path.Combine(_folderPath, $"{title}{Extension}");
	}

	/// <summary>
	/// タイトルがファイル名として安全に使えるかどうかを判定する。
	/// </summary>
	internal static bool IsValidTitle(string title) =>
		!string.IsNullOrWhiteSpace(title) &&
		title != "." &&
		title != ".." &&
		title.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
}
