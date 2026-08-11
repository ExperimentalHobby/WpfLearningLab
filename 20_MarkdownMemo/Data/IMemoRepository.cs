using MarkdownMemo.Models;

namespace MarkdownMemo.Data;

/// <summary>
/// メモ(Markdown文書)の永続化を担うリポジトリの抽象。
/// </summary>
public interface IMemoRepository
{
	/// <summary>
	/// 保存済みの全メモのサマリを、最終更新日時の降順で取得する。
	/// </summary>
	IReadOnlyList<MemoSummary> GetAll();

	/// <summary>
	/// 指定したタイトルのメモの内容(Markdown文字列)を読み込む。
	/// </summary>
	string Load(string title);

	/// <summary>
	/// 指定したタイトルでメモを保存する。同名のメモが既に存在する場合は上書きする。
	/// </summary>
	void Save(string title, string content);

	/// <summary>
	/// 指定したタイトルのメモを削除する。
	/// </summary>
	void Delete(string title);
}
