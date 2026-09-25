using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskApiWorkbench.Server.Data;

namespace TaskApiWorkbench.Server.Tests;

/// <summary>
/// エンドポイントの結合テスト用に、テストごとに独立したInMemoryデータベースへ差し替える<see cref="WebApplicationFactory{TEntryPoint}"/>。
/// </summary>
public class TaskApiFactory : WebApplicationFactory<Program>
{
	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		// AddDbContextのoptionsActionはDbContextOptionsがスコープ(リクエスト)ごとに解決されるたびに
		// 再実行される。Guid.NewGuid()をラムダ内で直接呼ぶとリクエストごとに別のDB名になってしまうため、
		// ここで1回だけ生成した値をキャプチャして使う。
		var databaseName = Guid.NewGuid().ToString();
		builder.ConfigureServices(services =>
		{
			services.RemoveAll<DbContextOptions<TaskDbContext>>();
			services.AddDbContext<TaskDbContext>(options => options.UseInMemoryDatabase(databaseName));
		});
	}
}
