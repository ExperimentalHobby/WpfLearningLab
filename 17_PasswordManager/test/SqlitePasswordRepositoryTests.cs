using Microsoft.Data.Sqlite;
using PasswordManager.Data;
using PasswordManager.Models;

namespace PasswordManager.Tests;

/// <summary>
/// <see cref="SqlitePasswordRepository"/> の単体テスト。
/// テストごとに一時ファイルへ実際にSQLiteでアクセスし、CRUDを検証する。
/// </summary>
public class SqlitePasswordRepositoryTests : IDisposable
{
	private readonly string _dbPath;

	public SqlitePasswordRepositoryTests()
	{
		_dbPath = Path.Combine(Path.GetTempPath(), $"PasswordManagerTests_{Guid.NewGuid():N}.db");
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
	/// パス条件: Addで登録したエントリがGetAllで取得できること
	/// </summary>
	[Fact]
	public void Add_登録したエントリがGetAllで取得できる()
	{
		var repository = new SqlitePasswordRepository(_dbPath);
		var entry = new PasswordEntry
		{
			Site = "example.com",
			Username = "user1",
			EncryptedPassword = "encrypted-value",
		};

		repository.Add(entry);
		var all = repository.GetAll();

		Assert.Single(all);
		Assert.Equal("example.com", all[0].Site);
		Assert.Equal("user1", all[0].Username);
		Assert.Equal("encrypted-value", all[0].EncryptedPassword);
		Assert.NotEqual(0, entry.Id);
	}

	/// <summary>
	/// パス条件: Updateで更新した内容がGetAllに反映されること
	/// </summary>
	[Fact]
	public void Update_更新した内容がGetAllに反映される()
	{
		var repository = new SqlitePasswordRepository(_dbPath);
		var entry = new PasswordEntry
		{
			Site = "example.com",
			Username = "user1",
			EncryptedPassword = "encrypted-value",
		};
		repository.Add(entry);

		entry.Site = "example.org";
		entry.Username = "user2";
		entry.EncryptedPassword = "new-encrypted-value";
		repository.Update(entry);
		var all = repository.GetAll();

		Assert.Single(all);
		Assert.Equal("example.org", all[0].Site);
		Assert.Equal("user2", all[0].Username);
		Assert.Equal("new-encrypted-value", all[0].EncryptedPassword);
	}

	/// <summary>
	/// パス条件: Deleteで削除したエントリがGetAllから消えること
	/// </summary>
	[Fact]
	public void Delete_削除したエントリがGetAllから消える()
	{
		var repository = new SqlitePasswordRepository(_dbPath);
		var entry = new PasswordEntry
		{
			Site = "example.com",
			Username = "user1",
			EncryptedPassword = "encrypted-value",
		};
		repository.Add(entry);

		repository.Delete(entry.Id);
		var all = repository.GetAll();

		Assert.Empty(all);
	}

	/// <summary>
	/// パス条件: 初期化前はIsInitializedがfalseを返すこと
	/// </summary>
	[Fact]
	public void IsInitialized_初期化前はfalseを返す()
	{
		var repository = new SqlitePasswordRepository(_dbPath);

		Assert.False(repository.IsInitialized());
	}

	/// <summary>
	/// パス条件: Initializeで保存したソルトと検証用値がGetSalt/GetVerificationValueで取得でき、IsInitializedがtrueになること
	/// </summary>
	[Fact]
	public void Initialize_保存した内容が取得できIsInitializedがtrueになる()
	{
		var repository = new SqlitePasswordRepository(_dbPath);
		var salt = new byte[] { 1, 2, 3, 4 };

		repository.Initialize(salt, "verification-value");

		Assert.True(repository.IsInitialized());
		Assert.Equal(salt, repository.GetSalt());
		Assert.Equal("verification-value", repository.GetVerificationValue());
	}
}
