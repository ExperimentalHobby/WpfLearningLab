using InvoicePrinter.Models;

namespace InvoicePrinter.Tests;

/// <summary>
/// <see cref="InvoiceCalculator"/> のテスト。
/// </summary>
public class InvoiceCalculatorTests
{
    /// <summary>
    /// パス条件: 複数明細の数量・単価から正しい小計(各行の数量×単価の合計)を計算すること
    /// </summary>
    [Fact]
    public void CalculateSubtotal_数量と単価から正しい小計を計算する()
    {
        var calculator = new InvoiceCalculator();
        var lines = new List<InvoiceLine>
        {
            new() { ItemName = "商品A", Quantity = 2, UnitPrice = 1000m },
            new() { ItemName = "商品B", Quantity = 3, UnitPrice = 500m },
        };

        var subtotal = calculator.CalculateSubtotal(lines);

        Assert.Equal(3500m, subtotal);
    }

    /// <summary>
    /// パス条件: 明細が空の場合、小計は0を返すこと
    /// </summary>
    [Fact]
    public void CalculateSubtotal_明細が空の場合は0を返す()
    {
        var calculator = new InvoiceCalculator();

        var subtotal = calculator.CalculateSubtotal(new List<InvoiceLine>());

        Assert.Equal(0m, subtotal);
    }

    /// <summary>
    /// パス条件: 小計に消費税率10%を掛けた際、円未満の端数を切り捨てること
    /// </summary>
    [Fact]
    public void CalculateTax_小計に対して消費税率10パーセントを掛け円未満を切り捨てる()
    {
        var calculator = new InvoiceCalculator();

        var tax = calculator.CalculateTax(1055m);

        Assert.Equal(105m, tax);
    }

    /// <summary>
    /// パス条件: 小計と消費税を合計した金額を返すこと
    /// </summary>
    [Fact]
    public void CalculateTotal_小計と消費税を合計する()
    {
        var calculator = new InvoiceCalculator();

        var total = calculator.CalculateTotal(1055m, 105m);

        Assert.Equal(1160m, total);
    }
}
