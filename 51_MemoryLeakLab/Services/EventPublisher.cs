namespace MemoryLeakLab.Services;

/// <summary>
/// アプリ全体で共有される長寿命のイベント発行元。
/// このインスタンスへの強参照購読を解除せずに放置すると、購読者オブジェクトが解放されなくなる
/// (メモリリークの再現に使う)。
/// </summary>
public class EventPublisher
{
	/// <summary>
	/// 何らかの変更が起きたことを通知するイベント。
	/// </summary>
	public event EventHandler<EventArgs>? SomethingChanged;

	/// <summary>
	/// <see cref="SomethingChanged"/> を発火する。
	/// </summary>
	public void RaiseSomethingChanged() => SomethingChanged?.Invoke(this, EventArgs.Empty);
}
