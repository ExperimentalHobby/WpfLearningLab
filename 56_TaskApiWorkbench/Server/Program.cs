using Microsoft.EntityFrameworkCore;
using TaskApiWorkbench.Server.Data;
using TaskApiWorkbench.Server.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5100");
builder.Services.AddDbContext<TaskDbContext>(options => options.UseInMemoryDatabase("TaskApiWorkbench"));

var app = builder.Build();

app.MapTaskEndpoints();

app.Run();

/// <summary>
/// トップレベルステートメントのエントリーポイントを、統合テストの<c>WebApplicationFactory&lt;Program&gt;</c>から
/// 参照できるようにするための部分クラス宣言。
/// </summary>
public partial class Program
{
}
