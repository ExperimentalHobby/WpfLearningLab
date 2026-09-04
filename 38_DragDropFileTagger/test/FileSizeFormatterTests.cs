using DragDropFileTagger.Services;

namespace DragDropFileTagger.Tests;

/// <summary>
/// <see cref="FileSizeFormatter"/>のテスト。
/// </summary>
public class FileSizeFormatterTests
{
	/// <summary>
	/// パス条件: 1024バイト未満はB単位で表示されること。
	/// </summary>
	[Fact]
	public void Format_1024バイト未満はB単位で表示される()
	{
		Assert.Equal("500 B", FileSizeFormatter.Format(500));
	}

	/// <summary>
	/// パス条件: 1024バイト以上はKB単位で表示されること。
	/// </summary>
	[Fact]
	public void Format_1024バイト以上はKB単位で表示される()
	{
		Assert.Equal("1.5 KB", FileSizeFormatter.Format(1536));
	}

	/// <summary>
	/// パス条件: 1048576バイト以上はMB単位で表示されること。
	/// </summary>
	[Fact]
	public void Format_1048576バイト以上はMB単位で表示される()
	{
		Assert.Equal("1 MB", FileSizeFormatter.Format(1024 * 1024));
	}
}
