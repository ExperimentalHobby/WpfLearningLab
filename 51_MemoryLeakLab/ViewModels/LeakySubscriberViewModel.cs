using MemoryLeakLab.Services;

namespace MemoryLeakLab.ViewModels;

/// <summary>
/// Bad版(リークあり): <see cref="EventPublisher"/>のイベントに強参照で購読するが、
/// 購読解除の手段を持たない。Publisher側の購読者リストに保持され続けるため、
/// このインスタンスへの外部参照を切ってもGCされない(意図的なメモリリークの再現)。
/// </summary>
public class LeakySubscriberViewModel
{
	/// <summary>
	/// <see cref="EventPublisher.SomethingChanged"/> を受信した回数。
	/// </summary>
	public int ReceivedCount { get; private set; }

	/// <summary>
	/// 指定したPublisherのイベントに強参照で購読する。
	/// </summary>
	/// <param name="publisher">購読対象のPublisher。</param>
	public LeakySubscriberViewModel(EventPublisher publisher)
	{
		publisher.SomethingChanged += OnChanged;
	}

	private void OnChanged(object? sender, EventArgs e) => ReceivedCount++;
}
