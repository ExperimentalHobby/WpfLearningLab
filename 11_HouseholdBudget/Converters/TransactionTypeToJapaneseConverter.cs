using System.Globalization;
using System.Windows.Data;
using HouseholdBudget.Models;

namespace HouseholdBudget.Converters;

/// <summary>
/// <see cref="TransactionType"/> を表示用の日本語文字列(収入/支出)に変換する。
/// </summary>
public class TransactionTypeToJapaneseConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		value is TransactionType.Income ? "収入" : "支出";

	/// <inheritdoc/>
	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		value is "収入" ? TransactionType.Income : TransactionType.Expense;
}
