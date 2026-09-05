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

	/// <summary>
	/// パス条件: 小数点をコンマで表すカルチャ(de-DE)でAddし、ピリオドで表すカルチャ
	/// (invariant)でGetAllしても、元の値のまま正しく読み込めること
	/// (カレントカルチャに依存したToString/Parseを使うと、保存時と読込時のカルチャが
	/// 異なる場合にFormatExceptionや値の破損が起きるため、常にInvariantCultureで
	/// 永続化する必要がある)。
	/// </summary>
	[Fact]
	public void Add_保存時と読込時でカルチャが異なってもGetAllで正しい値に復元される()
	{
		var originalCulture = System.Globalization.CultureInfo.CurrentCulture;
		var repository = new SqliteTransactionRepository(_dbPath);
		var transaction = new Transaction
		{
			Date = new DateTime(2026, 8, 1),
			Type = TransactionType.Expense,
			Category = "食費",
			Amount = 1234.5m,
		};

		try
		{
			// 小数点をコンマで表すカルチャで保存する。
			System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("de-DE");
			repository.Add(transaction);

			// 小数点をピリオドで表すカルチャで読み込む(実運用でもOS設定変更等であり得る)。
			System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
			var all = repository.GetAll();

			Assert.Equal(1234.5m, all[0].Amount);
		}
		finally
		{
			System.Globalization.CultureInfo.CurrentCulture = originalCulture;
		}
	}
}
