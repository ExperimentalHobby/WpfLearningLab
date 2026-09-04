using ContactManager.Data;
using ContactManager.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.Tests;

/// <summary>
/// <see cref="EfContactRepository"/>の単体テスト。
/// テストごとに一時ファイルへ実際にSQLite+EF Coreのマイグレーションでアクセスし、CRUDを検証する。
/// </summary>
public class EfContactRepositoryTests : IDisposable
{
	private readonly string _dbPath;

	public EfContactRepositoryTests()
	{
		_dbPath = Path.Combine(Path.GetTempPath(), $"ContactManagerTests_{Guid.NewGuid():N}.db");
	}

	public void Dispose()
	{
		SqliteConnection.ClearAllPools();
		if (File.Exists(_dbPath))
		{
			File.Delete(_dbPath);
		}
	}

	private EfContactRepository CreateRepository()
	{
		var optionsBuilder = new DbContextOptionsBuilder<ContactManagerDbContext>();
		optionsBuilder.UseSqlite($"Data Source={_dbPath}");
		var context = new ContactManagerDbContext(optionsBuilder.Options);
		context.Database.Migrate();
		return new EfContactRepository(context);
	}

	/// <summary>
	/// パス条件: Addで登録した連絡先がGetAllで取得できること。
	/// </summary>
	[Fact]
	public void Add_登録した連絡先がGetAllで取得できる()
	{
		var repository = CreateRepository();
		var contact = new Contact { Name = "山田太郎", PhoneNumber = "090-1111-2222", Email = "yamada@example.com" };

		repository.Add(contact);
		var all = repository.GetAll();

		Assert.Single(all);
		Assert.Equal("山田太郎", all[0].Name);
		Assert.NotEqual(0, contact.Id);
	}

	/// <summary>
	/// パス条件: Updateで変更した内容が永続化されること。
	/// </summary>
	[Fact]
	public void Update_変更内容が永続化される()
	{
		var repository = CreateRepository();
		var contact = new Contact { Name = "山田太郎", PhoneNumber = "090-1111-2222", Email = "yamada@example.com" };
		repository.Add(contact);

		contact.PhoneNumber = "090-9999-8888";
		repository.Update(contact);

		var reloaded = repository.GetAll().Single();
		Assert.Equal("090-9999-8888", reloaded.PhoneNumber);
	}

	/// <summary>
	/// パス条件: Deleteで削除した連絡先がGetAllから消えること。
	/// </summary>
	[Fact]
	public void Delete_削除した連絡先がGetAllから消える()
	{
		var repository = CreateRepository();
		var contact = new Contact { Name = "山田太郎" };
		repository.Add(contact);

		repository.Delete(contact.Id);

		Assert.Empty(repository.GetAll());
	}

	/// <summary>
	/// パス条件: GetAllは氏名の昇順で返すこと。
	/// (SQLiteの既定コレーションは符号位置順のため、曖昧さを避けアルファベット名で検証する)
	/// </summary>
	[Fact]
	public void GetAll_氏名の昇順で返す()
	{
		var repository = CreateRepository();
		repository.Add(new Contact { Name = "Suzuki" });
		repository.Add(new Contact { Name = "Abe" });

		var all = repository.GetAll();

		Assert.Equal("Abe", all[0].Name);
		Assert.Equal("Suzuki", all[1].Name);
	}
}
