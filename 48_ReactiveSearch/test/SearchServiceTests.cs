using ReactiveSearch.Services;

namespace ReactiveSearch.Tests;

/// <summary>
/// <see cref="SearchService"/> のテスト。
/// </summary>
public class SearchServiceTests
{
    /// <summary>
    /// パス条件: 候補一覧の中から検索文字列に部分一致(大文字小文字を区別しない)する候補を返すこと
    /// </summary>
    [Fact]
    public void Search_部分一致する候補を返す()
    {
        var service = new SearchService(new[] { "Apple", "Banana", "Grape" });

        var results = service.Search("an");

        Assert.Equal(new[] { "Banana" }, results);
    }

    /// <summary>
    /// パス条件: どの候補にも一致しない検索文字列の場合、空の一覧を返すこと
    /// </summary>
    [Fact]
    public void Search_一致する候補がない場合は空を返す()
    {
        var service = new SearchService(new[] { "Apple", "Banana", "Grape" });

        var results = service.Search("xyz");

        Assert.Empty(results);
    }
}
