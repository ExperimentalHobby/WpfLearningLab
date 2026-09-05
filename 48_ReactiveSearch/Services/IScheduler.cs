namespace ReactiveSearch.Services;

/// <summary>
/// 指定時間後にアクションを実行するスケジューラの抽象化。
/// テスト時はFakeに差し替え、実運用では <see cref="DispatcherTimerScheduler"/> を使用する。
/// </summary>
public interface IScheduler
{
    /// <summary>
    /// <paramref name="delay"/> 後に <paramref name="action"/> を実行するようスケジュールする。
    /// 戻り値の <see cref="IDisposable"/> を Dispose するとスケジュールをキャンセルできる。
    /// </summary>
    IDisposable Schedule(TimeSpan delay, Action action);
}
