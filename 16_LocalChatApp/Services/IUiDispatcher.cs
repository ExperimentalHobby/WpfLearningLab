namespace LocalChatApp.Services;

/// <summary>
/// バックグラウンドスレッドからUIスレッドで処理を実行するための抽象。
/// <see cref="TcpChatConnection"/>のイベントは受信用バックグラウンドスレッドから発火するため、
/// ViewModelの状態更新はこの抽象を介してUIスレッドにマーシャリングする。
/// </summary>
public interface IUiDispatcher
{
	/// <summary>
	/// 指定した処理をUIスレッドで実行する。
	/// </summary>
	void Invoke(Action action);
}
