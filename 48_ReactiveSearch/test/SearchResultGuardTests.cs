using ReactiveSearch.Services;

namespace ReactiveSearch.Tests;

/// <summary>
/// <see cref="SearchResultGuard"/> のテスト。
/// </summary>
public class SearchResultGuardTests
{
    /// <summary>
    /// パス条件: BeginRequestで発行したリクエスト番号が、他のリクエストが始まっていない
    /// 状態でIsCurrentに渡されるとtrueを返すこと
    /// </summary>
    [Fact]
    public void IsCurrent_最新のリクエスト番号の場合はtrueを返す()
    {
        var guard = new SearchResultGuard();

        var version = guard.BeginRequest();

        Assert.True(guard.IsCurrent(version));
    }

    /// <summary>
    /// パス条件: 古いリクエスト番号で判定すると、新しいリクエストが始まった後はfalseを返すこと
    /// (＝古い検索結果は無視される)
    /// </summary>
    [Fact]
    public void IsCurrent_古いリクエスト番号の場合はfalseを返す()
    {
        var guard = new SearchResultGuard();

        var firstVersion = guard.BeginRequest();
        guard.BeginRequest();

        Assert.False(guard.IsCurrent(firstVersion));
    }
}
