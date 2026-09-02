using FileOrganizer.Models;
using FileOrganizer.Services;

namespace FileOrganizer.Tests;

/// <summary>
/// <see cref="FileSortingRuleMatcher"/> の単体テスト。
/// </summary>
public class FileSortingRuleMatcherTests
{
	/// <summary>
	/// パス条件: 拡張子が一致するルールがある場合、その移動先フォルダ名を返すこと
	/// </summary>
	[Fact]
	public void GetDestinationFolder_拡張子が一致するルールの移動先を返す()
	{
		var rules = new List<SortingRule> { new(".jpg", "Images"), new(".pdf", "Documents") };

		var destination = FileSortingRuleMatcher.GetDestinationFolder(@"C:\Downloads\photo.jpg", rules);

		Assert.Equal("Images", destination);
	}

	/// <summary>
	/// パス条件: 一致するルールが無い場合はnullを返すこと
	/// </summary>
	[Fact]
	public void GetDestinationFolder_一致するルールが無い場合nullを返す()
	{
		var rules = new List<SortingRule> { new(".jpg", "Images") };

		var destination = FileSortingRuleMatcher.GetDestinationFolder(@"C:\Downloads\report.pdf", rules);

		Assert.Null(destination);
	}

	/// <summary>
	/// パス条件: 拡張子の大文字小文字を区別しないこと
	/// </summary>
	[Fact]
	public void GetDestinationFolder_拡張子の大文字小文字を区別しない()
	{
		var rules = new List<SortingRule> { new(".jpg", "Images") };

		var destination = FileSortingRuleMatcher.GetDestinationFolder(@"C:\Downloads\photo.JPG", rules);

		Assert.Equal("Images", destination);
	}
}
