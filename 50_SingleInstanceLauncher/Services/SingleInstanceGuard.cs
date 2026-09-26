namespace SingleInstanceLauncher.Services;

/// <summary>
/// 名前付きMutexを使って、自分が最初に起動したインスタンスかどうかを判定するガード。
/// </summary>
public class SingleInstanceGuard : IDisposable
{
    private readonly Mutex _mutex;

    /// <summary>自分が最初のインスタンスであれば true。</summary>
    public bool IsFirstInstance { get; }

    /// <summary>ガードを初期化する。</summary>
    /// <param name="mutexName">多重起動判定に使う名前付きMutexの名前。</param>
    public SingleInstanceGuard(string mutexName)
    {
        // 「自分が最初のインスタンスか」の判定にはcreatedNewのみを使い、Mutexの所有(取得)は
        // 行わない。所有した場合、Dispose元と異なるスレッドからReleaseMutexを呼ぶと
        // ApplicationExceptionになりうるが、名前付きMutexはプロセス内で参照されている限り
        // カーネルオブジェクトが存在し続けるため、所有しなくても多重起動検知は成立する。
        _mutex = new Mutex(initiallyOwned: false, name: mutexName, out var createdNew);
        IsFirstInstance = createdNew;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _mutex.Dispose();
    }
}
