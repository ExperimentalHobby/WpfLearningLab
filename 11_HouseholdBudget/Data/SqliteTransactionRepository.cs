using HouseholdBudget.Models;
using Microsoft.Data.Sqlite;

namespace HouseholdBudget.Data;

/// <summary>
/// SQLiteファイルを使って取引データをCRUDするリポジトリ。
/// </summary>
public class SqliteTransactionRepository : ITransactionRepository
{
	private readonly string _connectionString;

	/// <summary>
	/// リポジトリを初期化し、DBファイルとテーブルが無ければ作成する。
	/// </summary>
	/// <param name="dbPath">SQLiteファイルのパス。</param>
	public SqliteTransactionRepository(string dbPath)
	{
		_connectionString = $"Data Source={dbPath}";
		using var connection = OpenConnection();
		using var command = connection.CreateCommand();
		command.CommandText = """
			CREATE TABLE IF NOT EXISTS Transactions (
				Id INTEGER PRIMARY KEY AUTOINCREMENT,
				Date TEXT NOT NULL,
				Type INTEGER NOT NULL,
				Category TEXT NOT NULL,
				Amount TEXT NOT NULL,
				Memo TEXT NOT NULL
			)
			""";
		command.ExecuteNonQuery();
	}

	/// <inheritdoc/>
	public IReadOnlyList<Transaction> GetAll()
	{
		using var connection = OpenConnection();
		using var command = connection.CreateCommand();
		command.CommandText = "SELECT Id, Date, Type, Category, Amount, Memo FROM Transactions ORDER BY Date, Id";

		var result = new List<Transaction>();
		using var reader = command.ExecuteReader();
		while (reader.Read())
		{
			result.Add(new Transaction
			{
				Id = reader.GetInt32(0),
				Date = DateTime.Parse(reader.GetString(1)),
				Type = (TransactionType)reader.GetInt32(2),
				Category = reader.GetString(3),
				Amount = decimal.Parse(reader.GetString(4)),
				Memo = reader.GetString(5),
			});
		}

		return result;
	}

	/// <inheritdoc/>
	public void Add(Transaction transaction)
	{
		using var connection = OpenConnection();
		using var command = connection.CreateCommand();
		command.CommandText = """
			INSERT INTO Transactions (Date, Type, Category, Amount, Memo)
			VALUES ($date, $type, $category, $amount, $memo);
			SELECT last_insert_rowid();
			""";
		command.Parameters.AddWithValue("$date", transaction.Date.ToString("O"));
		command.Parameters.AddWithValue("$type", (int)transaction.Type);
		command.Parameters.AddWithValue("$category", transaction.Category);
		command.Parameters.AddWithValue("$amount", transaction.Amount.ToString());
		command.Parameters.AddWithValue("$memo", transaction.Memo);

		transaction.Id = Convert.ToInt32(command.ExecuteScalar());
	}

	/// <inheritdoc/>
	public void Delete(int id)
	{
		using var connection = OpenConnection();
		using var command = connection.CreateCommand();
		command.CommandText = "DELETE FROM Transactions WHERE Id = $id";
		command.Parameters.AddWithValue("$id", id);
		command.ExecuteNonQuery();
	}

	private SqliteConnection OpenConnection()
	{
		var connection = new SqliteConnection(_connectionString);
		connection.Open();
		return connection;
	}
}
