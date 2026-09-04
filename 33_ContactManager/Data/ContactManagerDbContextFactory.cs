using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ContactManager.Data;

/// <summary>
/// <c>dotnet ef migrations</c> 実行時(デザインタイム)専用に<see cref="ContactManagerDbContext"/>を生成するファクトリ。
/// アプリ実行時はDIコンテナ(<see cref="App"/>)経由で生成されるため、これは使われない。
/// </summary>
public class ContactManagerDbContextFactory : IDesignTimeDbContextFactory<ContactManagerDbContext>
{
	/// <inheritdoc/>
	public ContactManagerDbContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<ContactManagerDbContext>();
		optionsBuilder.UseSqlite("Data Source=design-time.db");
		return new ContactManagerDbContext(optionsBuilder.Options);
	}
}
