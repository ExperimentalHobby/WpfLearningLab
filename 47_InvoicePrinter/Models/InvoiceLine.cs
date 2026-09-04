namespace InvoicePrinter.Models;

/// <summary>
/// 請求書の明細1行(品目・数量・単価)。
/// </summary>
public class InvoiceLine
{
    /// <summary>品目名。</summary>
    public string ItemName { get; set; } = string.Empty;

    /// <summary>数量。</summary>
    public decimal Quantity { get; set; }

    /// <summary>単価。</summary>
    public decimal UnitPrice { get; set; }

    /// <summary>この行の金額(数量×単価)。</summary>
    public decimal Amount => Quantity * UnitPrice;
}
