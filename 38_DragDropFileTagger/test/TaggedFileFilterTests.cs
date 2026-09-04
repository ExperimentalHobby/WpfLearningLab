using DragDropFileTagger.Models;
using DragDropFileTagger.Services;

namespace DragDropFileTagger.Tests;

/// <summary>
/// <see cref="TaggedFileFilter"/>のテスト。
/// </summary>
public class TaggedFileFilterTests
{
	private static List<TaggedFile> CreateSampleFiles() =>
	[
		new() { FilePath = @"C:\a.txt", Tags = ["重要", "仕事"] },
		new() { FilePath = @"C:\b.txt", Tags = ["プライベート"] },
		new() { FilePath = @"C:\c.txt", Tags = ["重要"] },
	];

	/// <summary>
	/// パス条件: タグを指定すると、そのタグを持つファイルのみに絞り込まれること。
	/// </summary>
	[Fact]
	public void Filter_タグを指定するとそのタグを持つファイルのみ返す()
	{
		var result = TaggedFileFilter.Filter(CreateSampleFiles(), "重要");

		Assert.Equal(2, result.Count);
		Assert.All(result, file => Assert.Contains("重要", file.Tags));
	}

	/// <summary>
	/// パス条件: タグを指定しない(null/空)場合、全件返すこと。
	/// </summary>
	[Fact]
	public void Filter_タグ未指定の場合全件返す()
	{
		var files = CreateSampleFiles();

		Assert.Equal(3, TaggedFileFilter.Filter(files, null).Count);
		Assert.Equal(3, TaggedFileFilter.Filter(files, "").Count);
	}
}
