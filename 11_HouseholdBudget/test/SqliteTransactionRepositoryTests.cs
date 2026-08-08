using HouseholdBudget.Data;
using HouseholdBudget.Models;
using Microsoft.Data.Sqlite;

namespace HouseholdBudget.Tests;

/// <summary>
/// <see cref="SqliteTransactionRepository"/> の単体テスト。
/// テストごとに一時ファイルへ実際にSQLiteでアクセスし、CRUDを検証する。
/// </summary>
public class SqliteTransactionRepositoryTests : IDisposable
{
	private readonly string _dbPath;

	public SqliteTransactionRepositoryTests()
	{
		_dbPath = Path.Combine(Path.GetTempPath(), $"HouseholdBudgetTests_{Guid.NewGuid():N}.db");
	}

	public void Dispose()
	{
		// SqliteConnectionは既定でコネクションプーリングを行うため、
		// Dispose後もファイルハンドルが残り削除できないことがある。
		SqliteConnection.ClearAllPools();
		if (File.Exists(_dbPath))
		{
			File.Delete(_dbPath);
		}
	}

	/// <summary>
	/// パス条件: Addで取引を登録するとGetAllで取得できること
	/// </summary>
	[Fact]
	public void Add_登録した取引がGetAllで取得できる()
	{
		var repository = new SqliteTransactionRepository(_dbPath);
		var transaction = new Transaction
		{
			Date = new DateTime(2026, 8, 1),
			Type = TransactionType.Income,
			Category = "給与",
			Amount = 300000m,
			Memo = "8月分",
		};

		repository.Add(transaction);
		var all = repository.GetAll();

		Assert.Single(all);
		Assert.Equal("給与", all[0].Category);
		Assert.Equal(300000m, all[0].Amount);
		Assert.Equal("8月分", all[0].Memo);
		Assert.NotEqual(0, transaction.Id);
	}

	/// <summary>
	/// パス条件: Deleteで削除した取引がGetAllの結果から消えること
	/// </summary>
	[Fact]
	public void Delete_削除した取引がGetAllから消える()
	{
		var repository = new SqliteTransactionRepository(_dbPath);
		var transaction = new Transaction
		{
			Date = new DateTime(2026, 8, 1),
			Type = TransactionType.Expense,
			Category = "食費",
			Amount = 1500m,
		};
		repository.Add(transaction);

		repository.Delete(transaction.Id);
		var all = repository.GetAll();

		Assert.Empty(all);
	}
}
