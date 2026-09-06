using System.Globalization;
using System.Windows.Documents;
using InvoicePrinter.Models;

namespace InvoicePrinter.Tests;

/// <summary>
/// <see cref="FlowDocumentBuilder"/> のテスト。
/// </summary>
public class FlowDocumentBuilderTests
{
    /// <summary>
    /// パス条件: 明細2件を渡してBuildすると、テーブルに明細行数(2行)+ヘッダー行(1行)が含まれること
    /// </summary>
    [Fact]
    public void Build_明細行数分のテーブル行を含む()
    {
        var lines = new List<InvoiceLine>
        {
            new() { ItemName = "商品A", Quantity = 2, UnitPrice = 1000m },
            new() { ItemName = "商品B", Quantity = 3, UnitPrice = 500m },
        };

        var document = FlowDocumentBuilder.Build("山田商事", lines, 3500m, 350m, 3850m);

        var table = document.Blocks.OfType<Table>().Single();
        var rowCount = table.RowGroups.Sum(g => g.Rows.Count);
        Assert.Equal(3, rowCount);
    }

    /// <summary>
    /// パス条件: Buildしたドキュメントのテキストに顧客名と合計金額の文字列が含まれること
    /// </summary>
    [Fact]
    public void Build_顧客名と合計金額がドキュメントのテキストに含まれる()
    {
        var lines = new List<InvoiceLine> { new() { ItemName = "商品A", Quantity = 1, UnitPrice = 3850m } };

        var document = FlowDocumentBuilder.Build("山田商事", lines, 3850m, 385m, 4235m);

        var text = new TextRange(document.ContentStart, document.ContentEnd).Text;
        Assert.Contains("山田商事", text);
        Assert.Contains("4,235", text);
    }

    /// <summary>
    /// パス条件: 実行環境のカルチャに関わらず、金額は日本円表記("¥")になること
    /// (現在のカルチャに依存する"C0"書式のせいで、環境によっては"$"等になっていた不具合の回帰テスト)。
    /// </summary>
    [Fact]
    public void Build_現在のカルチャに関わらず日本円表記になる()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            var lines = new List<InvoiceLine> { new() { ItemName = "商品A", Quantity = 1, UnitPrice = 1000m } };

            var document = FlowDocumentBuilder.Build("山田商事", lines, 1000m, 100m, 1100m);

            var text = new TextRange(document.ContentStart, document.ContentEnd).Text;
            // ja-JPカルチャの通貨記号は環境によって"¥"(U+00A5)/"￥"(全角、U+FFE5)のいずれかになりうるため、
            // どちらであっても日本円表記になっていることを確認する。
            Assert.True(text.Contains('¥') || text.Contains('￥'), $"日本円表記が含まれていません: {text}");
            Assert.DoesNotContain("$", text);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}
