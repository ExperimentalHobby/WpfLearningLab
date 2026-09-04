using DragDropFileTagger.Models;

namespace DragDropFileTagger.Data;

/// <summary>
/// タグ付けファイル一覧(タグ・並び順)の永続化を担うリポジトリの抽象。
/// </summary>
public interface ITaggedFileRepository
{
	/// <summary>保存済みのファイル一覧を読み込む。保存が無い場合は空のリストを返す。</summary>
	List<TaggedFile> Load();

	/// <summary>ファイル一覧を保存する。</summary>
	void Save(IReadOnlyList<TaggedFile> files);
}
