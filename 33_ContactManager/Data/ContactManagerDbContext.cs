using ContactManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.Data;

/// <summary>
/// 連絡先管理アプリのSQLiteデータベースに対する<see cref="DbContext"/>。
/// </summary>
public class ContactManagerDbContext : DbContext
{
	/// <summary>
	/// コンテキストを初期化する。
	/// </summary>
	/// <param name="options">接続先等のオプション。</param>
	public ContactManagerDbContext(DbContextOptions<ContactManagerDbContext> options)
		: base(options)
	{
	}

	/// <summary>登録済みの連絡先。</summary>
	public DbSet<Contact> Contacts => Set<Contact>();
}
