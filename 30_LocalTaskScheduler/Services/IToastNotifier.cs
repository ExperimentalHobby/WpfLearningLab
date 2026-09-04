namespace LocalTaskScheduler.Services;

/// <summary>
/// Windowsトースト通知を表示する処理の抽象。
/// </summary>
public interface IToastNotifier
{
	/// <summary>
	/// トースト通知を表示する。
	/// </summary>
	/// <param name="title">通知のタイトル。</param>
	/// <param name="message">通知の本文。</param>
	void Show(string title, string message);
}
