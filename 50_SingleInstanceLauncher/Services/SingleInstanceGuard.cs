namespace SingleInstanceLauncher.Services;

/// <summary>
/// 名前付きMutexを使って、自分が最初に起動したインスタンスかどうかを判定するガード。
/// </summary>
public class SingleInstanceGuard : IDisposable
{
    private readonly Mutex _mutex;

    /// <summary>自分が最初のインスタンスであれば true。</summary>
    public bool IsFirstInstance { get; }

    public SingleInstanceGuard(string mutexName)
    {
        _mutex = new Mutex(initiallyOwned: true, name: mutexName, out var createdNew);
        IsFirstInstance = createdNew;
    }

    public void Dispose()
    {
        if (IsFirstInstance)
        {
            _mutex.ReleaseMutex();
        }

        _mutex.Dispose();
    }
}
