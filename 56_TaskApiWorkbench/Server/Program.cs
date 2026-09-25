using Microsoft.EntityFrameworkCore;
using TaskApiWorkbench.Server.Data;
using TaskApiWorkbench.Server.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5100");
builder.Services.AddDbContext<TaskDbContext>(options => options.UseInMemoryDatabase("TaskApiWorkbench"));

var app = builder.Build();

app.MapTaskEndpoints();

app.Run();

public partial class Program
{
}
