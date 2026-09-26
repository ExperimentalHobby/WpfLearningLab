using HouseholdBudget.Data;
using HouseholdBudget.Models;

namespace HouseholdBudget.Tests;

/// <summary>
/// <see cref="HouseholdBudget.ViewModels.MainViewModel"/> のテスト用に、メモリ上でCRUDを行う<see cref="ITransactionRepository"/>実装。
/// </summary>
public class FakeTransactionRepository : ITransactionRepository
{
	private readonly List<Transaction> _transactions = [];
	private int _nextId = 1;

	/// <summary>リポジトリを初期化する。</summary>
	/// <param name="initial">初期投入する取引データ(省略可)。</param>
	public FakeTransactionRepository(IEnumerable<Transaction>? initial = null)
	{
		if (initial is not null)
		{
			foreach (var transaction in initial)
			{
				transaction.Id = _nextId++;
				_transactions.Add(transaction);
			}
		}
	}

	/// <inheritdoc/>
	public IReadOnlyList<Transaction> GetAll() => _transactions.ToList();

	/// <inheritdoc/>
	public void Add(Transaction transaction)
	{
		transaction.Id = _nextId++;
		_transactions.Add(transaction);
	}

	/// <inheritdoc/>
	public void Delete(int id)
	{
		_transactions.RemoveAll(t => t.Id == id);
	}
}
