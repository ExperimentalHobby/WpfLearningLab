namespace PasswordManager.Services;

/// <summary>
/// 指定した遅延後にコールバックを実行するスケジューラの抽象。
/// クリップボードの自動クリア等、実時間の経過が絡む処理を実際に待たずに
/// テストできるようにするために、ViewModelから実行方法を切り離す。
/// </summary>
public interface IDelayedActionScheduler
{
	/// <summary>
	/// 指定した時間が経過した後に<paramref name="action"/>を実行する。
	/// </summary>
	/// <param name="delay">実行までの遅延時間。</param>
	/// <param name="action">実行するコールバック。</param>
	void Schedule(TimeSpan delay, Action action);
}
