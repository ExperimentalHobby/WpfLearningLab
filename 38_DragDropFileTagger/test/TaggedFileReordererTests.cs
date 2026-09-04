using DragDropFileTagger.Models;
using DragDropFileTagger.Services;

namespace DragDropFileTagger.Tests;

/// <summary>
/// <see cref="TaggedFileReorderer"/>のテスト。
/// </summary>
public class TaggedFileReordererTests
{
	private static List<TaggedFile> CreateSampleFiles() =>
	[
		new() { FilePath = "A", SortOrder = 0 },
		new() { FilePath = "B", SortOrder = 1 },
		new() { FilePath = "C", SortOrder = 2 },
	];

	/// <summary>
	/// パス条件: 先頭の要素を末尾へ移動すると、順序が正しく入れ替わること。
	/// </summary>
	[Fact]
	public void Move_先頭要素を末尾へ移動すると順序が入れ替わる()
	{
		var files = CreateSampleFiles();

		TaggedFileReorderer.Move(files, oldIndex: 0, newIndex: 2);

		Assert.Equal(["B", "C", "A"], files.Select(f => f.FilePath));
	}

	/// <summary>
	/// パス条件: 移動後、全要素のSortOrderが並び順通りに再採番されること。
	/// </summary>
	[Fact]
	public void Move_移動後SortOrderが並び順通りに再採番される()
	{
		var files = CreateSampleFiles();

		TaggedFileReorderer.Move(files, oldIndex: 2, newIndex: 0);

		Assert.Equal([0, 1, 2], files.Select(f => f.SortOrder));
		Assert.Equal("C", files[0].FilePath);
	}

	/// <summary>
	/// パス条件: 範囲外のインデックスを指定した場合、何も変化しないこと。
	/// </summary>
	[Fact]
	public void Move_範囲外のインデックスの場合何も変化しない()
	{
		var files = CreateSampleFiles();

		TaggedFileReorderer.Move(files, oldIndex: 0, newIndex: 99);

		Assert.Equal(["A", "B", "C"], files.Select(f => f.FilePath));
	}
}
