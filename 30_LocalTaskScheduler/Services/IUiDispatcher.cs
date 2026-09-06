namespace LocalTaskScheduler.Services;

/// <summary>
/// バックグラウンドスレッドからUIスレッドへ処理をマーシャリングする抽象。
/// <see cref="System.Threading.Timer"/>のコールバックはスレッドプールのスレッドで実行されるため、
/// UIバインド対象(<see cref="System.Collections.ObjectModel.ObservableCollection{T}"/>等)の更新に使う。
/// </summary>
public interface IUiDispatcher
{
	/// <summary>
	/// 指定した処理をUIスレッドで実行する。呼び出し元のスレッドをブロックしない
	/// (UIスレッドでの実行完了を待たずに戻る)。呼び出し元がスレッドプールの
	/// タイマーコールバック等の場合、同期的な<c>Dispatcher.Invoke</c>で待たせ続けると
	/// スレッドプールのスレッドを不必要に塞いでしまうため。
	/// </summary>
	/// <param name="action">実行する処理。</param>
	void Invoke(Action action);
}
