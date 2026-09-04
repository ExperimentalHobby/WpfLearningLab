using System.Windows.Input;
using AccessibleNoteApp.Services;

namespace AccessibleNoteApp.Tests;

/// <summary>
/// <see cref="MemoListNavigator"/> の単体テスト。
/// </summary>
public class MemoListNavigatorTests
{
	/// <summary>
	/// パス条件: Downキーで選択が1つ後ろに進むこと
	/// </summary>
	[Fact]
	public void GetNextIndex_Downキーで1つ後ろに進む()
	{
		var result = MemoListNavigator.GetNextIndex(0, 3, Key.Down);

		Assert.Equal(1, result);
	}

	/// <summary>
	/// パス条件: 最後の項目でDownキーを押しても末尾でクランプされること
	/// </summary>
	[Fact]
	public void GetNextIndex_末尾でDownキーを押しても末尾のまま()
	{
		var result = MemoListNavigator.GetNextIndex(2, 3, Key.Down);

		Assert.Equal(2, result);
	}

	/// <summary>
	/// パス条件: 未選択(-1)の状態でDownキーを押すと先頭が選択されること
	/// </summary>
	[Fact]
	public void GetNextIndex_未選択の状態でDownキーを押すと先頭が選択される()
	{
		var result = MemoListNavigator.GetNextIndex(-1, 3, Key.Down);

		Assert.Equal(0, result);
	}

	/// <summary>
	/// パス条件: Upキーで選択が1つ前に進むこと
	/// </summary>
	[Fact]
	public void GetNextIndex_Upキーで1つ前に進む()
	{
		var result = MemoListNavigator.GetNextIndex(1, 3, Key.Up);

		Assert.Equal(0, result);
	}

	/// <summary>
	/// パス条件: 先頭の項目でUpキーを押しても先頭でクランプされること
	/// </summary>
	[Fact]
	public void GetNextIndex_先頭でUpキーを押しても先頭のまま()
	{
		var result = MemoListNavigator.GetNextIndex(0, 3, Key.Up);

		Assert.Equal(0, result);
	}

	/// <summary>
	/// パス条件: Homeキーで先頭が選択されること
	/// </summary>
	[Fact]
	public void GetNextIndex_Homeキーで先頭が選択される()
	{
		var result = MemoListNavigator.GetNextIndex(2, 3, Key.Home);

		Assert.Equal(0, result);
	}

	/// <summary>
	/// パス条件: Endキーで末尾が選択されること
	/// </summary>
	[Fact]
	public void GetNextIndex_Endキーで末尾が選択される()
	{
		var result = MemoListNavigator.GetNextIndex(0, 3, Key.End);

		Assert.Equal(2, result);
	}

	/// <summary>
	/// パス条件: ナビゲーション対象外のキーの場合nullを返すこと
	/// </summary>
	[Fact]
	public void GetNextIndex_対象外のキーの場合nullを返す()
	{
		var result = MemoListNavigator.GetNextIndex(0, 3, Key.A);

		Assert.Null(result);
	}

	/// <summary>
	/// パス条件: 件数0の場合はどのキーでも常にnullを返すこと
	/// </summary>
	[Fact]
	public void GetNextIndex_件数0の場合は常にnullを返す()
	{
		var result = MemoListNavigator.GetNextIndex(-1, 0, Key.Down);

		Assert.Null(result);
	}
}
