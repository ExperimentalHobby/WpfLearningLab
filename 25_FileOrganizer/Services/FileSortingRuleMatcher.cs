using System.IO;
using FileOrganizer.Models;

namespace FileOrganizer.Services;

/// <summary>
/// ファイルパスと振り分けルール一覧から、該当する移動先フォルダ名を求める純粋なロジック。
/// </summary>
public static class FileSortingRuleMatcher
{
	/// <summary>
	/// 指定したファイルの拡張子に一致するルールを探し、移動先フォルダ名を返す。
	/// 一致するルールが無い場合は<see langword="null"/>を返す。
	/// </summary>
	/// <param name="filePath">対象ファイルのパス。</param>
	/// <param name="rules">振り分けルール一覧。</param>
	public static string? GetDestinationFolder(string filePath, IReadOnlyList<SortingRule> rules)
	{
		var extension = Path.GetExtension(filePath);
		var rule = rules.FirstOrDefault(r => string.Equals(r.Extension, extension, StringComparison.OrdinalIgnoreCase));
		return rule?.DestinationFolderName;
	}
}
