using Microsoft.EntityFrameworkCore;
using TaskApiWorkbench.Server.Data;
using TaskApiWorkbench.Server.Dtos;
using TaskApiWorkbench.Server.Models;

namespace TaskApiWorkbench.Server.Endpoints;

/// <summary>
/// タスクCRUD用のエンドポイントを登録する。
/// </summary>
public static class TaskEndpoints
{
	/// <summary>
	/// "/api/tasks"配下にタスクCRUDのエンドポイントをマッピングする。
	/// </summary>
	/// <param name="app">エンドポイントを登録するアプリケーション。</param>
	public static void MapTaskEndpoints(this WebApplication app)
	{
		var group = app.MapGroup("/api/tasks");

		group.MapGet("/", GetAllAsync);
		group.MapGet("/{id:int}", GetByIdAsync);
		group.MapPost("/", CreateAsync);
		group.MapPut("/{id:int}", UpdateAsync);
		group.MapPatch("/{id:int}/complete", ToggleCompleteAsync);
		group.MapDelete("/{id:int}", DeleteAsync);
	}

	private static async Task<IResult> GetAllAsync(TaskDbContext db)
	{
		var tasks = await db.Tasks.AsNoTracking().OrderBy(t => t.Id).ToListAsync();
		return Results.Ok(tasks.Select(ToDto));
	}

	private static async Task<IResult> GetByIdAsync(int id, TaskDbContext db)
	{
		var task = await db.Tasks.FindAsync(id);
		return task is null ? Results.NotFound() : Results.Ok(ToDto(task));
	}

	private static async Task<IResult> CreateAsync(TaskItemRequest request, TaskDbContext db)
	{
		if (string.IsNullOrWhiteSpace(request.Title))
		{
			return TitleRequiredResult();
		}

		var task = new TaskItem { Title = request.Title, Description = request.Description };
		db.Tasks.Add(task);
		await db.SaveChangesAsync();
		return Results.Created($"/api/tasks/{task.Id}", ToDto(task));
	}

	private static async Task<IResult> UpdateAsync(int id, TaskItemRequest request, TaskDbContext db)
	{
		if (string.IsNullOrWhiteSpace(request.Title))
		{
			return TitleRequiredResult();
		}

		var task = await db.Tasks.FindAsync(id);
		if (task is null)
		{
			return Results.NotFound();
		}

		task.Title = request.Title;
		task.Description = request.Description;
		await db.SaveChangesAsync();
		return Results.Ok(ToDto(task));
	}

	private static async Task<IResult> ToggleCompleteAsync(int id, TaskDbContext db)
	{
		var task = await db.Tasks.FindAsync(id);
		if (task is null)
		{
			return Results.NotFound();
		}

		task.IsCompleted = !task.IsCompleted;
		await db.SaveChangesAsync();
		return Results.Ok(ToDto(task));
	}

	private static async Task<IResult> DeleteAsync(int id, TaskDbContext db)
	{
		var task = await db.Tasks.FindAsync(id);
		if (task is null)
		{
			return Results.NotFound();
		}

		db.Tasks.Remove(task);
		await db.SaveChangesAsync();
		return Results.NoContent();
	}

	private static IResult TitleRequiredResult() =>
		Results.ValidationProblem(new Dictionary<string, string[]>
		{
			["Title"] = ["タイトルは必須です。"],
		});

	private static TaskItemDto ToDto(TaskItem task) => new()
	{
		Id = task.Id,
		Title = task.Title,
		Description = task.Description,
		IsCompleted = task.IsCompleted,
	};
}
