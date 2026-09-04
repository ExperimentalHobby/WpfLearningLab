using DragDropFileTagger.Models;

namespace DragDropFileTagger.Services;

/// <summary>
/// タグを指定してファイル一覧を絞り込む。
/// </summary>
public static class TaggedFileFilter
{
	/// <summary>
	/// <paramref name="tag"/>が空/nullの場合は全件、指定がある場合はそのタグを持つファイルのみを返す。
	/// </summary>
	public static IReadOnlyList<TaggedFile> Filter(IReadOnlyList<TaggedFile> files, string? tag)
	{
		if (string.IsNullOrWhiteSpace(tag))
		{
			return files;
		}
		return files.Where(file => file.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase)).ToList();
	}
}
