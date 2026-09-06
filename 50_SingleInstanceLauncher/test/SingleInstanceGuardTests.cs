using SingleInstanceLauncher.Services;

namespace SingleInstanceLauncher.Tests;

/// <summary>
/// <see cref="SingleInstanceGuard"/> のテスト。
/// </summary>
public class SingleInstanceGuardTests
{
    /// <summary>
    /// パス条件: 同じ名前で1つ目のガードを作成すると、IsFirstInstanceがtrueになること
    /// </summary>
    [Fact]
    public void Constructor_1つ目はIsFirstInstanceがtrueになる()
    {
        var mutexName = $"Local\\SingleInstanceLauncherTests_{Guid.NewGuid():N}";
        using var guard = new SingleInstanceGuard(mutexName);

        Assert.True(guard.IsFirstInstance);
    }

    /// <summary>
    /// パス条件: 1つ目のガードが生きている間に同じ名前で2つ目を作成すると、
    /// IsFirstInstanceがfalseになること
    /// </summary>
    [Fact]
    public void Constructor_1つ目が生きている間の2つ目はIsFirstInstanceがfalseになる()
    {
        var mutexName = $"Local\\SingleInstanceLauncherTests_{Guid.NewGuid():N}";
        using var first = new SingleInstanceGuard(mutexName);

        using var second = new SingleInstanceGuard(mutexName);

        Assert.False(second.IsFirstInstance);
    }

    /// <summary>
    /// パス条件: 1つ目のガードをDisposeした後に同じ名前で新しいガードを作成すると、
    /// 再びIsFirstInstanceがtrueになること
    /// </summary>
    [Fact]
    public void Dispose後に同じ名前で作成すると再びIsFirstInstanceがtrueになる()
    {
        var mutexName = $"Local\\SingleInstanceLauncherTests_{Guid.NewGuid():N}";
        var first = new SingleInstanceGuard(mutexName);
        first.Dispose();

        using var second = new SingleInstanceGuard(mutexName);

        Assert.True(second.IsFirstInstance);
    }

    /// <summary>
    /// パス条件: Disposeを呼んでも例外にならないこと
    /// (initiallyOwned:trueで取得した状態で異なるスレッドからReleaseMutexするとApplicationExceptionに
    /// なりうる不具合の回帰テスト。取得(所有)自体をしない実装であれば、どのスレッドからDisposeしても
    /// 例外にならないはず)。
    /// </summary>
    [Fact]
    public void Dispose_例外にならない()
    {
        var mutexName = $"Local\\SingleInstanceLauncherTests_{Guid.NewGuid():N}";
        var guard = new SingleInstanceGuard(mutexName);

        Exception? exception = null;
        var thread = new Thread(() =>
        {
            try
            {
                guard.Dispose();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        });
        thread.Start();
        thread.Join();

        Assert.Null(exception);
    }
}
