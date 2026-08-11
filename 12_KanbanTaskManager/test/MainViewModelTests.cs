using KanbanTaskManager.Models;
using KanbanTaskManager.ViewModels;

namespace KanbanTaskManager.Tests;

/// <summary>
/// <see cref="MainViewModel"/> の単体テスト。
/// </summary>
public class MainViewModelTests
{
	/// <summary>
	/// パス条件: 生成時にTodo/InProgress/Doneの3カラムが用意されること
	/// </summary>
	[Fact]
	public void コンストラクタ_Todo_InProgress_Doneの3カラムが用意される()
	{
		var viewModel = new MainViewModel();

		Assert.Equal(KanbanStatus.Todo, viewModel.TodoColumn.Status);
		Assert.Equal(KanbanStatus.InProgress, viewModel.InProgressColumn.Status);
		Assert.Equal(KanbanStatus.Done, viewModel.DoneColumn.Status);
	}

	/// <summary>
	/// パス条件: MoveTaskで指定タスクが移動元カラムから消え、移動先カラムに追加され、Statusが更新されること
	/// </summary>
	[Fact]
	public void MoveTask_タスクが移動元から移動先へ移り_Statusが更新される()
	{
		var viewModel = new MainViewModel();
		var task = new TaskItem { Title = "設計資料を書く", Status = KanbanStatus.Todo };
		viewModel.TodoColumn.Tasks.Add(task);

		viewModel.MoveTask(task, KanbanStatus.InProgress);

		Assert.DoesNotContain(task, viewModel.TodoColumn.Tasks);
		Assert.Contains(task, viewModel.InProgressColumn.Tasks);
		Assert.Equal(KanbanStatus.InProgress, task.Status);
	}

	/// <summary>
	/// パス条件: MoveTaskCommandを実行すると、MoveTaskと同じくカラム間移動が行われること(ビヘイビアからの橋渡し用コマンド)
	/// </summary>
	[Fact]
	public void MoveTaskCommand_実行するとタスクが移動先カラムへ移動する()
	{
		var viewModel = new MainViewModel();
		var task = new TaskItem { Title = "設計資料を書く", Status = KanbanStatus.Todo };
		viewModel.TodoColumn.Tasks.Add(task);

		viewModel.MoveTaskCommand.Execute(new MoveTaskRequest(task, KanbanStatus.Done));

		Assert.DoesNotContain(task, viewModel.TodoColumn.Tasks);
		Assert.Contains(task, viewModel.DoneColumn.Tasks);
		Assert.Equal(KanbanStatus.Done, task.Status);
	}
}
