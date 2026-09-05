using System.Runtime.CompilerServices;
using MemoryLeakLab.Services;
using MemoryLeakLab.ViewModels;

namespace MemoryLeakLab.Tests;

public class EventPublisherSubscriptionTests
{
	/// <summary>
	/// パス条件: Publisherがイベントを発火すると、生存中のBad版購読者のReceivedCountが増えること。
	/// </summary>
	[Fact]
	public void LeakySubscriber_ReceivesEvent_WhileAlive()
	{
		var publisher = new EventPublisher();
		var subscriber = new LeakySubscriberViewModel(publisher);

		publisher.RaiseSomethingChanged();

		Assert.Equal(1, subscriber.ReceivedCount);
	}

	/// <summary>
	/// パス条件: Publisherがイベントを発火すると、生存中のGood版購読者のReceivedCountが増えること。
	/// </summary>
	[Fact]
	public void WeakSubscriber_ReceivesEvent_WhileAlive()
	{
		var publisher = new EventPublisher();
		var subscriber = new WeakSubscriberViewModel(publisher);

		publisher.RaiseSomethingChanged();

		Assert.Equal(1, subscriber.ReceivedCount);
	}

	/// <summary>
	/// パス条件: Bad版(強参照購読)は、ローカル参照を切ってGCしてもPublisherに保持され続けるため、
	/// CountAlive()が0にならないこと(リークの再現)。
	/// </summary>
	[Fact]
	public void LeakySubscriber_NotCollected_AfterReferenceReleasedAndGc()
	{
		var publisher = new EventPublisher();
		var tracker = new LeakTracker();

		TrackLeakySubscriber(publisher, tracker);

		GC.Collect();
		GC.WaitForPendingFinalizers();
		GC.Collect();

		Assert.Equal(1, tracker.CountAlive());
	}

	/// <summary>
	/// パス条件: Good版(WeakEventManager購読)は、ローカル参照を切ってGCすると解放され、
	/// CountAlive()が0になること(修正版での解消確認)。
	/// </summary>
	[Fact]
	public void WeakSubscriber_Collected_AfterReferenceReleasedAndGc()
	{
		var publisher = new EventPublisher();
		var tracker = new LeakTracker();

		TrackWeakSubscriber(publisher, tracker);

		GC.Collect();
		GC.WaitForPendingFinalizers();
		GC.Collect();

		Assert.Equal(0, tracker.CountAlive());
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void TrackLeakySubscriber(EventPublisher publisher, LeakTracker tracker)
	{
		var subscriber = new LeakySubscriberViewModel(publisher);
		tracker.Track(subscriber);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void TrackWeakSubscriber(EventPublisher publisher, LeakTracker tracker)
	{
		var subscriber = new WeakSubscriberViewModel(publisher);
		tracker.Track(subscriber);
	}
}
