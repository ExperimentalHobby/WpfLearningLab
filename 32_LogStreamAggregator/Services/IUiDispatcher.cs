namespace LogStreamAggregator.Services;

/// <summary>
/// バックグラウンドスレッドからUIスレッドへ処理をマーシャリングする抽象。
/// Producer/Consumerのバックグラウンドタスクから、UIバインド対象のプロパティ更新に使う。
/// </summary>
public interface IUiDispatcher
{
	/// <summary>
	/// 指定した処理をUIスレッドで実行する。
	/// </summary>
	void Invoke(Action action);
}
