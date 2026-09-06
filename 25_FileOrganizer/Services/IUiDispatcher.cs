namespace FileOrganizer.Services;

/// <summary>
/// バックグラウンドスレッドからUIスレッドへ処理をマーシャリングする抽象。
/// <see cref="System.IO.FileSystemWatcher"/>のイベントはバックグラウンドスレッドで発火するため、
/// UIバインド対象(<see cref="System.Collections.ObjectModel.ObservableCollection{T}"/>等)の更新に使う。
/// </summary>
public interface IUiDispatcher
{
	/// <summary>
	/// 指定した処理をUIスレッドで実行する。
	/// </summary>
	/// <param name="action">実行する処理。</param>
	void Invoke(Action action);

	/// <summary>
	/// 指定した処理をUIスレッドで実行し、結果を返す。<see cref="System.Collections.ObjectModel.ObservableCollection{T}"/>等、
	/// UIスレッド以外からの読み取りが安全でないコレクションのスナップショット取得に使う。
	/// </summary>
	/// <param name="func">実行する処理。</param>
	T Invoke<T>(Func<T> func);
}
