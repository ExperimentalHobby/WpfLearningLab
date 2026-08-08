using HouseholdBudget.Models;

namespace HouseholdBudget.Data;

/// <summary>
/// 取引データの永続化を担うリポジトリの抽象。
/// </summary>
public interface ITransactionRepository
{
	/// <summary>
	/// 登録済みの全取引を取得する。
	/// </summary>
	IReadOnlyList<Transaction> GetAll();

	/// <summary>
	/// 取引を新規追加する。追加後、<paramref name="transaction"/> の <see cref="Transaction.Id"/> が更新される。
	/// </summary>
	void Add(Transaction transaction);

	/// <summary>
	/// 指定したIDの取引を削除する。
	/// </summary>
	void Delete(int id);
}
