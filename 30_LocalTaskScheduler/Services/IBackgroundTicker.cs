namespace LocalTaskScheduler.Services;

/// <summary>
/// バックグラウンドで一定間隔ごとに処理を発火させる仕組みの抽象。
/// </summary>
public interface IBackgroundTicker : IDisposable
{
	/// <summary>
	/// 一定間隔ごとに発火するイベント。バックグラウンドスレッドで発火する。
	/// </summary>
	event Action<DateTime>? Ticked;

	/// <summary>
	/// 発火を開始する。
	/// </summary>
	/// <param name="interval">発火間隔。</param>
	void Start(TimeSpan interval);

	/// <summary>
	/// 発火を停止する。
	/// </summary>
	void Stop();
}
