using Microsoft.EntityFrameworkCore;
using TaskApiWorkbench.Server.Models;

namespace TaskApiWorkbench.Server.Data;

/// <summary>
/// タスクを永続化するEF CoreのDbContext。学習用のためInMemoryプロバイダを使用する。
/// </summary>
public sealed class TaskDbContext : DbContext
{
	/// <summary>
	/// DbContextを初期化する。
	/// </summary>
	/// <param name="options">接続先プロバイダ等の設定。</param>
	public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
	{
	}

	/// <summary>タスクのテーブル。</summary>
	public DbSet<TaskItem> Tasks => Set<TaskItem>();
}
