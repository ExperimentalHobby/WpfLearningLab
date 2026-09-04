namespace LocalTaskScheduler.Services;

/// <summary>
/// バックグラウンドスレッドからUIスレッドへ処理をマーシャリングする抽象。
/// <see cref="System.Threading.Timer"/>のコールバックはスレッドプールのスレッドで実行されるため、
/// UIバインド対象(<see cref="System.Collections.ObjectModel.ObservableCollection{T}"/>等)の更新に使う。
/// </summary>
public interface IUiDispatcher
{
	/// <summary>
	/// 指定した処理をUIスレッドで実行する。
	/// </summary>
	/// <param name="action">実行する処理。</param>
	void Invoke(Action action);
}
