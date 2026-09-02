using MusicPlayer.Models;
using MusicPlayer.Services;

namespace MusicPlayer.Tests;

/// <summary>
/// <see cref="PlaylistNavigator"/> の単体テスト。リピート/シャッフルの組み合わせで
/// 次/前に再生すべきトラックのindexが正しく求まることを検証する。
/// </summary>
public class PlaylistNavigatorTests
{
	/// <summary>
	/// パス条件: 通常時(リピートOFF・シャッフルOFF)は次のindexが1つ進むこと
	/// </summary>
	[Fact]
	public void GetNextIndex_通常時は次のindexが1つ進む()
	{
		var next = PlaylistNavigator.GetNextIndex(5, 1, RepeatMode.Off, isShuffle: false, new Random());

		Assert.Equal(2, next);
	}

	/// <summary>
	/// パス条件: リピートOFFで最後の曲の次はnull(再生すべき曲なし)になること
	/// </summary>
	[Fact]
	public void GetNextIndex_リピートOFFで末尾の次はnullになる()
	{
		var next = PlaylistNavigator.GetNextIndex(5, 4, RepeatMode.Off, isShuffle: false, new Random());

		Assert.Null(next);
	}

	/// <summary>
	/// パス条件: リピート全曲で最後の曲の次は先頭(0)に折り返すこと
	/// </summary>
	[Fact]
	public void GetNextIndex_リピート全曲で末尾の次は先頭に折り返す()
	{
		var next = PlaylistNavigator.GetNextIndex(5, 4, RepeatMode.All, isShuffle: false, new Random());

		Assert.Equal(0, next);
	}

	/// <summary>
	/// パス条件: リピート1曲の場合、次のindexは現在と同じになること
	/// </summary>
	[Fact]
	public void GetNextIndex_リピート1曲の場合現在と同じindexになる()
	{
		var next = PlaylistNavigator.GetNextIndex(5, 2, RepeatMode.One, isShuffle: false, new Random());

		Assert.Equal(2, next);
	}

	/// <summary>
	/// パス条件: シャッフル有効時は現在と異なるindexが返ること
	/// </summary>
	[Fact]
	public void GetNextIndex_シャッフル有効時は現在と異なるindexになる()
	{
		var next = PlaylistNavigator.GetNextIndex(5, 2, RepeatMode.Off, isShuffle: true, new Random(1));

		Assert.NotNull(next);
		Assert.NotEqual(2, next);
	}

	/// <summary>
	/// パス条件: 曲数が0件の場合はnullを返すこと
	/// </summary>
	[Fact]
	public void GetNextIndex_曲数0件の場合nullを返す()
	{
		var next = PlaylistNavigator.GetNextIndex(0, -1, RepeatMode.Off, isShuffle: false, new Random());

		Assert.Null(next);
	}

	/// <summary>
	/// パス条件: 通常時(リピートOFF・シャッフルOFF)は前のindexが1つ戻ること
	/// </summary>
	[Fact]
	public void GetPreviousIndex_通常時は前のindexが1つ戻る()
	{
		var previous = PlaylistNavigator.GetPreviousIndex(5, 2, RepeatMode.Off, isShuffle: false, new Random());

		Assert.Equal(1, previous);
	}

	/// <summary>
	/// パス条件: リピートOFFで先頭の前はnullになること
	/// </summary>
	[Fact]
	public void GetPreviousIndex_リピートOFFで先頭の前はnullになる()
	{
		var previous = PlaylistNavigator.GetPreviousIndex(5, 0, RepeatMode.Off, isShuffle: false, new Random());

		Assert.Null(previous);
	}

	/// <summary>
	/// パス条件: リピート全曲で先頭の前は末尾に折り返すこと
	/// </summary>
	[Fact]
	public void GetPreviousIndex_リピート全曲で先頭の前は末尾に折り返す()
	{
		var previous = PlaylistNavigator.GetPreviousIndex(5, 0, RepeatMode.All, isShuffle: false, new Random());

		Assert.Equal(4, previous);
	}

	/// <summary>
	/// パス条件: 曲数が1件のみでシャッフル有効な場合、常に0を返すこと(無限ループしない)
	/// </summary>
	[Fact]
	public void GetNextIndex_曲数1件でシャッフル有効でも無限ループせず0を返す()
	{
		var next = PlaylistNavigator.GetNextIndex(1, 0, RepeatMode.Off, isShuffle: true, new Random());

		Assert.Equal(0, next);
	}
}
