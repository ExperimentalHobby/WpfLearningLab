using System.Runtime.CompilerServices;
using MemoryLeakLab.ViewModels;

namespace MemoryLeakLab.Tests;

public class LeakTrackerTests
{
	/// <summary>
	/// パス条件: Trackでオブジェクトを登録するとTotalCountが登録数分増えること。
	/// </summary>
	[Fact]
	public void Track_IncreasesTotalCount()
	{
		var tracker = new LeakTracker();

		tracker.Track(new object());
		tracker.Track(new object());

		Assert.Equal(2, tracker.TotalCount);
	}

	/// <summary>
	/// パス条件: 追跡対象への参照を保持している間はCountAlive()がTotalCountと一致すること。
	/// </summary>
	[Fact]
	public void CountAlive_WhileReferenceHeld_EqualsTotalCount()
	{
		var tracker = new LeakTracker();
		var target = new object();

		tracker.Track(target);

		Assert.Equal(tracker.TotalCount, tracker.CountAlive());
		GC.KeepAlive(target);
	}

	/// <summary>
	/// パス条件: 追跡対象への参照を切ってGCを実行すると、通常のオブジェクトはCountAlive()が0になること。
	/// </summary>
	[Fact]
	public void CountAlive_AfterReferenceReleasedAndGc_BecomesZero()
	{
		var tracker = new LeakTracker();

		TrackDisposableTarget(tracker);

		GC.Collect();
		GC.WaitForPendingFinalizers();
		GC.Collect();

		Assert.Equal(0, tracker.CountAlive());
	}

	/// <summary>
	/// ローカル変数のスコープをテストメソッド本体から分離し、JITによる生存期間延長の影響を避ける。
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void TrackDisposableTarget(LeakTracker tracker)
	{
		var target = new object();
		tracker.Track(target);
	}
}
