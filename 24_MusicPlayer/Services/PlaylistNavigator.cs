using MusicPlayer.Models;

namespace MusicPlayer.Services;

/// <summary>
/// リピート/シャッフルの設定を考慮して、次/前に再生すべきトラックのindexを求める純粋なロジック。
/// <see cref="System.Windows.Controls.MediaElement"/>等の実際の再生処理には依存しない。
/// </summary>
public static class PlaylistNavigator
{
	/// <summary>
	/// 次に再生すべきトラックのindexを求める。再生すべきトラックが無い場合は<see langword="null"/>。
	/// </summary>
	/// <param name="count">プレイリストの曲数。</param>
	/// <param name="currentIndex">現在再生中のindex。</param>
	/// <param name="repeatMode">リピートモード。</param>
	/// <param name="isShuffle">シャッフルが有効かどうか。</param>
	/// <param name="random">シャッフル時に使う乱数生成器。</param>
	public static int? GetNextIndex(int count, int currentIndex, RepeatMode repeatMode, bool isShuffle, Random random)
	{
		if (count == 0)
		{
			return null;
		}

		if (repeatMode == RepeatMode.One)
		{
			return currentIndex;
		}

		if (isShuffle)
		{
			return PickRandomExcludingCurrent(count, currentIndex, random);
		}

		var nextIndex = currentIndex + 1;
		if (nextIndex < count)
		{
			return nextIndex;
		}

		return repeatMode == RepeatMode.All ? 0 : null;
	}

	/// <summary>
	/// 前に再生すべきトラックのindexを求める。再生すべきトラックが無い場合は<see langword="null"/>。
	/// </summary>
	/// <param name="count">プレイリストの曲数。</param>
	/// <param name="currentIndex">現在再生中のindex。</param>
	/// <param name="repeatMode">リピートモード。</param>
	/// <param name="isShuffle">シャッフルが有効かどうか。</param>
	/// <param name="random">シャッフル時に使う乱数生成器。</param>
	public static int? GetPreviousIndex(int count, int currentIndex, RepeatMode repeatMode, bool isShuffle, Random random)
	{
		if (count == 0)
		{
			return null;
		}

		if (repeatMode == RepeatMode.One)
		{
			return currentIndex;
		}

		if (isShuffle)
		{
			return PickRandomExcludingCurrent(count, currentIndex, random);
		}

		var previousIndex = currentIndex - 1;
		if (previousIndex >= 0)
		{
			return previousIndex;
		}

		return repeatMode == RepeatMode.All ? count - 1 : null;
	}

	private static int PickRandomExcludingCurrent(int count, int currentIndex, Random random)
	{
		if (count == 1)
		{
			return 0;
		}

		int candidate;
		do
		{
			candidate = random.Next(count);
		}
		while (candidate == currentIndex);

		return candidate;
	}
}
