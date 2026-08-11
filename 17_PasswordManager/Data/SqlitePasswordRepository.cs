using Microsoft.Data.Sqlite;
using PasswordManager.Models;

namespace PasswordManager.Data;

/// <summary>
/// SQLiteファイルを使ってパスワードエントリとマスターキー設定をCRUDするリポジトリ。
/// エントリ(Entries)とマスターキー設定(Settings、1行のみ)を同一DBファイルで管理する。
/// </summary>
public class SqlitePasswordRepository : IPasswordEntryRepository, IMasterKeyStore
{
	private readonly string _connectionString;

	/// <summary>
	/// リポジトリを初期化し、DBファイルとテーブルが無ければ作成する。
	/// </summary>
	/// <param name="dbPath">SQLiteファイルのパス。</param>
	public SqlitePasswordRepository(string dbPath)
	{
		_connectionString = $"Data Source={dbPath}";
		using var connection = OpenConnection();
		using var command = connection.CreateCommand();
		command.CommandText = """
			CREATE TABLE IF NOT EXISTS Entries (
				Id INTEGER PRIMARY KEY AUTOINCREMENT,
				Site TEXT NOT NULL,
				Username TEXT NOT NULL,
				EncryptedPassword TEXT NOT NULL
			);
			CREATE TABLE IF NOT EXISTS Settings (
				Id INTEGER PRIMARY KEY CHECK (Id = 1),
				Salt TEXT NOT NULL,
				VerificationValue TEXT NOT NULL
			)
			""";
		command.ExecuteNonQuery();
	}

	/// <inheritdoc/>
	public IReadOnlyList<PasswordEntry> GetAll()
	{
		using var connection = OpenConnection();
		using var command = connection.CreateCommand();
		command.CommandText = "SELECT Id, Site, Username, EncryptedPassword FROM Entries ORDER BY Id";

		var result = new List<PasswordEntry>();
		using var reader = command.ExecuteReader();
		while (reader.Read())
		{
			result.Add(new PasswordEntry
			{
				Id = reader.GetInt32(0),
				Site = reader.GetString(1),
				Username = reader.GetString(2),
				EncryptedPassword = reader.GetString(3),
			});
		}

		return result;
	}

	/// <inheritdoc/>
	public void Add(PasswordEntry entry)
	{
		using var connection = OpenConnection();
		using var command = connection.CreateCommand();
		command.CommandText = """
			INSERT INTO Entries (Site, Username, EncryptedPassword)
			VALUES ($site, $username, $encryptedPassword);
			SELECT last_insert_rowid();
			""";
		command.Parameters.AddWithValue("$site", entry.Site);
		command.Parameters.AddWithValue("$username", entry.Username);
		command.Parameters.AddWithValue("$encryptedPassword", entry.EncryptedPassword);

		entry.Id = Convert.ToInt32(command.ExecuteScalar());
	}

	/// <inheritdoc/>
	public void Update(PasswordEntry entry)
	{
		using var connection = OpenConnection();
		using var command = connection.CreateCommand();
		command.CommandText = """
			UPDATE Entries
			SET Site = $site, Username = $username, EncryptedPassword = $encryptedPassword
			WHERE Id = $id
			""";
		command.Parameters.AddWithValue("$site", entry.Site);
		command.Parameters.AddWithValue("$username", entry.Username);
		command.Parameters.AddWithValue("$encryptedPassword", entry.EncryptedPassword);
		command.Parameters.AddWithValue("$id", entry.Id);
		command.ExecuteNonQuery();
	}

	/// <inheritdoc/>
	public void Delete(int id)
	{
		using var connection = OpenConnection();
		using var command = connection.CreateCommand();
		command.CommandText = "DELETE FROM Entries WHERE Id = $id";
		command.Parameters.AddWithValue("$id", id);
		command.ExecuteNonQuery();
	}

	/// <inheritdoc/>
	public bool IsInitialized()
	{
		using var connection = OpenConnection();
		using var command = connection.CreateCommand();
		command.CommandText = "SELECT COUNT(*) FROM Settings WHERE Id = 1";
		return Convert.ToInt32(command.ExecuteScalar()) > 0;
	}

	/// <inheritdoc/>
	public byte[] GetSalt()
	{
		using var connection = OpenConnection();
		using var command = connection.CreateCommand();
		command.CommandText = "SELECT Salt FROM Settings WHERE Id = 1";
		var result = command.ExecuteScalar() ?? throw new InvalidOperationException("マスターキーが初期設定されていません。");
		return Convert.FromBase64String((string)result);
	}

	/// <inheritdoc/>
	public string GetVerificationValue()
	{
		using var connection = OpenConnection();
		using var command = connection.CreateCommand();
		command.CommandText = "SELECT VerificationValue FROM Settings WHERE Id = 1";
		var result = command.ExecuteScalar() ?? throw new InvalidOperationException("マスターキーが初期設定されていません。");
		return (string)result;
	}

	/// <inheritdoc/>
	public void Initialize(byte[] salt, string verificationValue)
	{
		using var connection = OpenConnection();
		using var command = connection.CreateCommand();
		command.CommandText = """
			INSERT INTO Settings (Id, Salt, VerificationValue)
			VALUES (1, $salt, $verificationValue)
			""";
		command.Parameters.AddWithValue("$salt", Convert.ToBase64String(salt));
		command.Parameters.AddWithValue("$verificationValue", verificationValue);
		command.ExecuteNonQuery();
	}

	private SqliteConnection OpenConnection()
	{
		var connection = new SqliteConnection(_connectionString);
		connection.Open();
		return connection;
	}
}
