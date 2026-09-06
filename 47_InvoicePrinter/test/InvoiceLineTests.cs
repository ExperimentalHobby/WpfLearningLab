using System.ComponentModel;
using InvoicePrinter.Models;

namespace InvoicePrinter.Tests;

/// <summary>
/// <see cref="InvoiceLine"/> のテスト。
/// </summary>
public class InvoiceLineTests
{
    /// <summary>
    /// パス条件: Quantityを変更すると、自身とAmountの両方についてPropertyChangedが発火すること
    /// (DataGridで数量を編集しても金額列が更新されない不具合の回帰テスト)。
    /// </summary>
    [Fact]
    public void Quantity_変更するとAmountの変更通知も発火する()
    {
        var line = new InvoiceLine { Quantity = 1, UnitPrice = 1000m };
        var raisedProperties = new List<string?>();
        line.PropertyChanged += (_, e) => raisedProperties.Add(e.PropertyName);

        line.Quantity = 2;

        Assert.Contains(nameof(InvoiceLine.Quantity), raisedProperties);
        Assert.Contains(nameof(InvoiceLine.Amount), raisedProperties);
    }

    /// <summary>
    /// パス条件: UnitPriceを変更すると、自身とAmountの両方についてPropertyChangedが発火すること
    /// </summary>
    [Fact]
    public void UnitPrice_変更するとAmountの変更通知も発火する()
    {
        var line = new InvoiceLine { Quantity = 1, UnitPrice = 1000m };
        var raisedProperties = new List<string?>();
        line.PropertyChanged += (_, e) => raisedProperties.Add(e.PropertyName);

        line.UnitPrice = 2000m;

        Assert.Contains(nameof(InvoiceLine.UnitPrice), raisedProperties);
        Assert.Contains(nameof(InvoiceLine.Amount), raisedProperties);
    }

    /// <summary>
    /// パス条件: 値が変化しない場合はPropertyChangedが発火しないこと
    /// </summary>
    [Fact]
    public void Quantity_同じ値を設定した場合は変更通知が発火しない()
    {
        var line = new InvoiceLine { Quantity = 1, UnitPrice = 1000m };
        var raised = false;
        line.PropertyChanged += (_, _) => raised = true;

        line.Quantity = 1;

        Assert.False(raised);
    }
}
