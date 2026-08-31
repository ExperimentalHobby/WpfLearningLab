using DragDropFileTagger.Models;

namespace DragDropFileTagger.Services;

/// <summary>
/// リスト内でのアイテムのドラッグ並び替えを行う。
/// </summary>
public static class TaggedFileReorderer
{
	/// <summary>
	/// <paramref name="files"/>内の<paramref name="oldIndex"/>番目の要素を<paramref name="newIndex"/>番目に移動し、
	/// 全要素の<see cref="TaggedFile.SortOrder"/>を並び順に再採番する。
	/// </summary>
	public static void Move(List<TaggedFile> files, int oldIndex, int newIndex)
	{
		if (oldIndex == newIndex
			|| oldIndex < 0 || oldIndex >= files.Count
			|| newIndex < 0 || newIndex >= files.Count)
		{
			return;
		}

		var item = files[oldIndex];
		files.RemoveAt(oldIndex);
		files.Insert(newIndex, item);

		for (var i = 0; i < files.Count; i++)
		{
			files[i].SortOrder = i;
		}
	}
}
