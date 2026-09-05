using System.Windows;
using MemoryLeakLab.Services;

namespace MemoryLeakLab.ViewModels;

/// <summary>
/// Good版(修正済み): <see cref="WeakEventManager{TEventSource, TEventArgs}"/>を使って
/// <see cref="EventPublisher"/>のイベントに弱参照で購読する。Publisher側は購読者を弱参照テーブルで
/// 保持するため、このインスタンスへの外部参照を切ればGC対象になる。
/// </summary>
public class WeakSubscriberViewModel
{
	/// <summary>
	/// <see cref="EventPublisher.SomethingChanged"/> を受信した回数。
	/// </summary>
	public int ReceivedCount { get; private set; }

	/// <summary>
	/// 指定したPublisherのイベントに<see cref="WeakEventManager{TEventSource, TEventArgs}"/>経由で購読する。
	/// </summary>
	/// <param name="publisher">購読対象のPublisher。</param>
	public WeakSubscriberViewModel(EventPublisher publisher)
	{
		WeakEventManager<EventPublisher, EventArgs>.AddHandler(
			publisher, nameof(EventPublisher.SomethingChanged), OnChanged);
	}

	private void OnChanged(object? sender, EventArgs e) => ReceivedCount++;
}
