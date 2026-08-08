namespace HouseholdBudget.Models;

/// <summary>
/// 取引の種別(収入・支出)。
/// </summary>
public enum TransactionType
{
	/// <summary>収入。</summary>
	Income,

	/// <summary>支出。</summary>
	Expense,
}

/// <summary>
/// 1件の収支取引を表すエンティティ。
/// </summary>
public class Transaction
{
	/// <summary>取引ID(SQLiteの自動採番)。新規作成時は0。</summary>
	public int Id { get; set; }

	/// <summary>取引日。</summary>
	public required DateTime Date { get; set; }

	/// <summary>種別(収入/支出)。</summary>
	public required TransactionType Type { get; set; }

	/// <summary>カテゴリ名(例: 食費、給与)。</summary>
	public required string Category { get; set; }

	/// <summary>金額。常に正の値で保持する。</summary>
	public required decimal Amount { get; set; }

	/// <summary>メモ。</summary>
	public string Memo { get; set; } = string.Empty;

	/// <summary>
	/// 収支計算用の符号付き金額。収入は正、支出は負として返す。
	/// </summary>
	public decimal SignedAmount => Type == TransactionType.Income ? Amount : -Amount;
}
