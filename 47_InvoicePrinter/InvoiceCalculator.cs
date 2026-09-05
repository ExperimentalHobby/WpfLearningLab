using InvoicePrinter.Models;

namespace InvoicePrinter;

/// <summary>
/// 請求書の小計・消費税・合計金額を計算するロジック。
/// </summary>
public class InvoiceCalculator
{
    /// <summary>消費税率(10%)。</summary>
    public const decimal TaxRate = 0.10m;

    /// <summary>
    /// 明細一覧から小計(各行の数量×単価の合計)を計算する。
    /// </summary>
    public decimal CalculateSubtotal(IReadOnlyList<InvoiceLine> lines)
    {
        return lines.Sum(line => line.Quantity * line.UnitPrice);
    }

    /// <summary>
    /// 小計に消費税率(<see cref="TaxRate"/>)を掛けて消費税額を計算する。円未満は切り捨てる。
    /// </summary>
    public decimal CalculateTax(decimal subtotal)
    {
        return Math.Floor(subtotal * TaxRate);
    }

    /// <summary>
    /// 小計と消費税を合計する。
    /// </summary>
    public decimal CalculateTotal(decimal subtotal, decimal tax)
    {
        return subtotal + tax;
    }
}
