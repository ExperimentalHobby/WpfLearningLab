using KanbanTaskManager.Models;
using KanbanTaskManager.ViewModels;

namespace KanbanTaskManager.Tests;

/// <summary>
/// <see cref="TaskColumnViewModel"/> の単体テスト。
/// </summary>
public class TaskColumnViewModelTests
{
	/// <summary>
	/// パス条件: AddCommand実行で、指定したStatusのタスクがTasksに追加されること
	/// </summary>
	[Fact]
	public void AddCommand_実行するとカラムのStatusでタスクが追加される()
	{
		var column = new TaskColumnViewModel("Todo", KanbanStatus.Todo)
		{
			NewTaskTitle = "設計資料を書く",
		};

		column.AddCommand.Execute(null);

		var task = Assert.Single(column.Tasks);
		Assert.Equal("設計資料を書く", task.Title);
		Assert.Equal(KanbanStatus.Todo, task.Status);
	}

	/// <summary>
	/// パス条件: AddCommand実行後、NewTaskTitleがクリアされること
	/// </summary>
	[Fact]
	public void AddCommand_実行後にNewTaskTitleがクリアされる()
	{
		var column = new TaskColumnViewModel("Todo", KanbanStatus.Todo)
		{
			NewTaskTitle = "設計資料を書く",
		};

		column.AddCommand.Execute(null);

		Assert.Equal(string.Empty, column.NewTaskTitle);
	}

	/// <summary>
	/// パス条件: NewTaskTitleが空白の場合、AddCommandが実行不可になること
	/// </summary>
	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	public void AddCommand_タイトルが空白の場合CanExecuteがfalseになる(string title)
	{
		var column = new TaskColumnViewModel("Todo", KanbanStatus.Todo)
		{
			NewTaskTitle = title,
		};

		var canExecute = column.AddCommand.CanExecute(null);

		Assert.False(canExecute);
	}

	/// <summary>
	/// パス条件: DeleteCommand実行で、指定したタスクがTasksから削除されること
	/// </summary>
	[Fact]
	public void DeleteCommand_実行すると指定タスクが削除される()
	{
		var column = new TaskColumnViewModel("Todo", KanbanStatus.Todo);
		var task = new TaskItem { Title = "設計資料を書く", Status = KanbanStatus.Todo };
		column.Tasks.Add(task);

		column.DeleteCommand.Execute(task);

		Assert.Empty(column.Tasks);
	}
}
