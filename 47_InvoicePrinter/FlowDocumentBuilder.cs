using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using InvoicePrinter.Models;

namespace InvoicePrinter;

/// <summary>
/// 請求書の明細・金額から <see cref="FlowDocument"/> を組み立てるロジック。
/// </summary>
public static class FlowDocumentBuilder
{
    /// <summary>
    /// 金額の書式に使うカルチャ。実行環境の現在のカルチャに依存すると通貨記号が変わってしまうため、
    /// 日本円前提のこのアプリでは常に明示的にja-JPを使う。
    /// </summary>
    private static readonly CultureInfo JapaneseCulture = CultureInfo.GetCultureInfo("ja-JP");

    /// <summary>
    /// 顧客名・明細一覧・小計・消費税・合計から請求書のFlowDocumentを生成する。
    /// </summary>
    public static FlowDocument Build(
        string customerName,
        IReadOnlyList<InvoiceLine> lines,
        decimal subtotal,
        decimal tax,
        decimal total)
    {
        var document = new FlowDocument
        {
            PagePadding = new Thickness(40),
            ColumnWidth = double.PositiveInfinity,
        };

        document.Blocks.Add(new Paragraph(new Run("請求書"))
        {
            FontSize = 22,
            FontWeight = FontWeights.Bold,
        });

        document.Blocks.Add(new Paragraph(new Run($"{customerName} 様")) { Margin = new Thickness(0, 0, 0, 16) });

        document.Blocks.Add(BuildTable(lines));

        document.Blocks.Add(new Paragraph(new Run($"小計: {subtotal.ToString("C0", JapaneseCulture)}")) { Margin = new Thickness(0, 16, 0, 0) });
        document.Blocks.Add(new Paragraph(new Run($"消費税: {tax.ToString("C0", JapaneseCulture)}")));
        document.Blocks.Add(new Paragraph(new Run($"合計: {total.ToString("C0", JapaneseCulture)}")) { FontWeight = FontWeights.Bold });

        return document;
    }

    private static Table BuildTable(IReadOnlyList<InvoiceLine> lines)
    {
        var table = new Table();
        for (var i = 0; i < 4; i++)
        {
            table.Columns.Add(new TableColumn());
        }

        var rowGroup = new TableRowGroup();
        rowGroup.Rows.Add(BuildRow("品目", "数量", "単価", "金額", isHeader: true));

        foreach (var line in lines)
        {
            rowGroup.Rows.Add(BuildRow(
                line.ItemName,
                line.Quantity.ToString("0.##"),
                line.UnitPrice.ToString("C0", JapaneseCulture),
                line.Amount.ToString("C0", JapaneseCulture),
                isHeader: false));
        }

        table.RowGroups.Add(rowGroup);
        return table;
    }

    private static TableRow BuildRow(string col1, string col2, string col3, string col4, bool isHeader)
    {
        var row = new TableRow();
        foreach (var text in new[] { col1, col2, col3, col4 })
        {
            var run = new Run(text);
            if (isHeader)
            {
                run.FontWeight = FontWeights.Bold;
            }

            row.Cells.Add(new TableCell(new Paragraph(run)) { Padding = new Thickness(4) });
        }

        return row;
    }
}
