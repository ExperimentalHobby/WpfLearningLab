using DataWorkbench.Models;

namespace DataWorkbench.Tests;

/// <summary>
/// <see cref="CsvFilterEngine"/> のテスト。
/// </summary>
public class CsvFilterEngineTests
{
    /// <summary>
    /// パス条件: 検索文字列がいずれかの列の値に部分一致する場合はtrueを返すこと
    /// </summary>
    [Fact]
    public void Matches_検索文字列がいずれかの列に部分一致する場合はtrueを返す()
    {
        var row = new CsvRow { ["名前"] = "山田太郎", ["年齢"] = "30" };

        var result = CsvFilterEngine.Matches(row, "山田");

        Assert.True(result);
    }

    /// <summary>
    /// パス条件: 検索文字列がどの列の値にも一致しない場合はfalseを返すこと
    /// </summary>
    [Fact]
    public void Matches_検索文字列がどの列にも一致しない場合はfalseを返す()
    {
        var row = new CsvRow { ["名前"] = "山田太郎", ["年齢"] = "30" };

        var result = CsvFilterEngine.Matches(row, "鈴木");

        Assert.False(result);
    }

    /// <summary>
    /// パス条件: 検索文字列が空文字列の場合は常にtrueを返すこと(フィルタなし)
    /// </summary>
    [Fact]
    public void Matches_検索文字列が空の場合は常にtrueを返す()
    {
        var row = new CsvRow { ["名前"] = "山田太郎" };

        var result = CsvFilterEngine.Matches(row, "");

        Assert.True(result);
    }
}
