using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace InvoicePrinter.Models;

/// <summary>
/// 請求書の明細1行(品目・数量・単価)。
/// </summary>
public class InvoiceLine : INotifyPropertyChanged
{
    private string _itemName = string.Empty;
    private decimal _quantity;
    private decimal _unitPrice;

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>品目名。</summary>
    public string ItemName
    {
        get => _itemName;
        set => SetProperty(ref _itemName, value);
    }

    /// <summary>数量。</summary>
    public decimal Quantity
    {
        get => _quantity;
        set
        {
            if (SetProperty(ref _quantity, value))
            {
                OnPropertyChanged(nameof(Amount));
            }
        }
    }

    /// <summary>単価。</summary>
    public decimal UnitPrice
    {
        get => _unitPrice;
        set
        {
            if (SetProperty(ref _unitPrice, value))
            {
                OnPropertyChanged(nameof(Amount));
            }
        }
    }

    /// <summary>この行の金額(数量×単価)。</summary>
    public decimal Amount => Quantity * UnitPrice;

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
