using TaskApiWorkbench.Client.Models;
using TaskApiWorkbench.Client.ViewModels;

namespace TaskApiWorkbench.Client.Tests;

/// <summary>
/// <see cref="MainViewModel"/> の単体テスト。
/// </summary>
public class MainViewModelTests
{
	/// <summary>
	/// パス条件: LoadCommand実行でAPIから取得したタスクがTasksに反映されること。
	/// </summary>
	[Fact]
	public async Task LoadCommand_成功時にTasksへタスク一覧が反映される()
	{
		var fake = new FakeTaskApiClient
		{
			TasksResult = [new TaskItem { Id = 1, Title = "資料整理" }],
		};
		var viewModel = new MainViewModel(fake);

		viewModel.LoadCommand.Execute(null);
		await Task.Delay(50);

		Assert.Single(viewModel.Tasks);
		Assert.Equal("資料整理", viewModel.Tasks[0].Title);
		Assert.Equal(string.Empty, viewModel.ErrorMessage);
	}

	/// <summary>
	/// パス条件: LoadCommand失敗時にErrorMessageが設定されること。
	/// </summary>
	[Fact]
	public async Task LoadCommand_失敗時にErrorMessageが設定される()
	{
		var fake = new FakeTaskApiClient { ExceptionToThrow = new HttpRequestException("接続失敗") };
		var viewModel = new MainViewModel(fake);

		viewModel.LoadCommand.Execute(null);
		await Task.Delay(50);

		Assert.NotEqual(string.Empty, viewModel.ErrorMessage);
		Assert.Empty(viewModel.Tasks);
	}

	/// <summary>
	/// パス条件: AddCommand実行で作成されたタスクがTasksに追加され、入力欄がクリアされること。
	/// </summary>
	[Fact]
	public async Task AddCommand_成功時にTasksへ追加され入力欄がクリアされる()
	{
		var fake = new FakeTaskApiClient
		{
			CreateResult = new TaskItem { Id = 1, Title = "資料整理" },
		};
		var viewModel = new MainViewModel(fake)
		{
			TitleInput = "資料整理",
			DescriptionInput = "月次分",
		};

		viewModel.AddCommand.Execute(null);
		await Task.Delay(50);

		Assert.Single(viewModel.Tasks);
		Assert.Equal(string.Empty, viewModel.TitleInput);
		Assert.Equal(string.Empty, viewModel.DescriptionInput);
	}

	/// <summary>
	/// パス条件: AddCommand失敗時にErrorMessageが設定され、Tasksに追加されないこと。
	/// </summary>
	[Fact]
	public async Task AddCommand_失敗時にErrorMessageが設定されTasksに追加されない()
	{
		var fake = new FakeTaskApiClient { ExceptionToThrow = new HttpRequestException("接続失敗") };
		var viewModel = new MainViewModel(fake) { TitleInput = "資料整理" };

		viewModel.AddCommand.Execute(null);
		await Task.Delay(50);

		Assert.NotEqual(string.Empty, viewModel.ErrorMessage);
		Assert.Empty(viewModel.Tasks);
	}

	/// <summary>
	/// パス条件: SelectedTaskを設定するとTitleInput/DescriptionInputに反映されること。
	/// </summary>
	[Fact]
	public void SelectedTask_設定するとTitleInputとDescriptionInputに反映される()
	{
		var viewModel = new MainViewModel(new FakeTaskApiClient());
		var task = new TaskItem { Id = 1, Title = "資料整理", Description = "月次分" };

		viewModel.SelectedTask = task;

		Assert.Equal("資料整理", viewModel.TitleInput);
		Assert.Equal("月次分", viewModel.DescriptionInput);
	}

	/// <summary>
	/// パス条件: UpdateCommand実行でTasks内の該当タスクが更新後の内容に置き換わり、
	/// SelectedTaskも更新後のタスクを指し続けること。
	/// (ObservableCollection要素を新しい参照に置き換えるとListBoxのSelectedItemバインディングが
	/// 古い参照を見失って選択解除されるため、明示的に選択を維持する必要がある)
	/// </summary>
	[Fact]
	public async Task UpdateCommand_成功時にTasks内の該当タスクが置き換わりSelectedTaskも更新される()
	{
		var original = new TaskItem { Id = 1, Title = "資料整理" };
		var updated = new TaskItem { Id = 1, Title = "資料整理(更新)", Description = "追記" };
		var fake = new FakeTaskApiClient
		{
			TasksResult = [original],
			UpdateResult = updated,
		};
		var viewModel = new MainViewModel(fake);
		viewModel.LoadCommand.Execute(null);
		await Task.Delay(50);
		viewModel.SelectedTask = viewModel.Tasks[0];
		viewModel.TitleInput = "資料整理(更新)";
		viewModel.DescriptionInput = "追記";

		viewModel.UpdateCommand.Execute(null);
		await Task.Delay(50);

		Assert.Equal("資料整理(更新)", viewModel.Tasks[0].Title);
		Assert.Same(updated, viewModel.SelectedTask);
		Assert.Equal(1, fake.LastUpdateCall!.Value.Id);
	}

	/// <summary>
	/// パス条件: ToggleCompleteCommand実行でTasks内の該当タスクの完了状態が置き換わり、
	/// SelectedTaskも更新後のタスクを指し続けること。
	/// </summary>
	[Fact]
	public async Task ToggleCompleteCommand_成功時にTasks内の該当タスクが置き換わりSelectedTaskも更新される()
	{
		var original = new TaskItem { Id = 1, Title = "資料整理", IsCompleted = false };
		var toggled = new TaskItem { Id = 1, Title = "資料整理", IsCompleted = true };
		var fake = new FakeTaskApiClient
		{
			TasksResult = [original],
			ToggleCompleteResult = toggled,
		};
		var viewModel = new MainViewModel(fake);
		viewModel.LoadCommand.Execute(null);
		await Task.Delay(50);
		viewModel.SelectedTask = viewModel.Tasks[0];

		viewModel.ToggleCompleteCommand.Execute(null);
		await Task.Delay(50);

		Assert.True(viewModel.Tasks[0].IsCompleted);
		Assert.Same(toggled, viewModel.SelectedTask);
		Assert.Equal(1, fake.LastToggledId);
	}

	/// <summary>
	/// パス条件: UpdateCommand完了(IsBusyがfalseに戻った)後に、DeleteCommandのCanExecuteChangedが
	/// 発火し、その時点でCanExecuteがtrueになっていること。
	/// (IsBusyの変更を他コマンドに通知しないと、最後に発火したCanExecuteChangedの時点では
	/// まだIsBusy=trueのままで、WPFのButton.IsEnabledがfalseに固まってしまう)
	/// </summary>
	[Fact]
	public async Task UpdateCommand_完了後の最後のCanExecuteChanged時点でDeleteCommandが実行可能になっている()
	{
		var task = new TaskItem { Id = 1, Title = "資料整理" };
		var fake = new FakeTaskApiClient
		{
			TasksResult = [task],
			UpdateResult = new TaskItem { Id = 1, Title = "資料整理(更新)" },
		};
		var viewModel = new MainViewModel(fake);
		viewModel.LoadCommand.Execute(null);
		await Task.Delay(50);
		viewModel.SelectedTask = viewModel.Tasks[0];

		var canExecuteAtLastRaise = false;
		viewModel.DeleteCommand.CanExecuteChanged += (_, _) => canExecuteAtLastRaise = viewModel.DeleteCommand.CanExecute(null);

		viewModel.UpdateCommand.Execute(null);
		await Task.Delay(50);

		Assert.True(canExecuteAtLastRaise);
	}

	/// <summary>
	/// パス条件: DeleteCommand実行でTasksから該当タスクが削除されSelectedTaskがnullになること。
	/// </summary>
	[Fact]
	public async Task DeleteCommand_成功時にTasksから削除されSelectedTaskがnullになる()
	{
		var task = new TaskItem { Id = 1, Title = "資料整理" };
		var fake = new FakeTaskApiClient { TasksResult = [task] };
		var viewModel = new MainViewModel(fake);
		viewModel.LoadCommand.Execute(null);
		await Task.Delay(50);
		viewModel.SelectedTask = viewModel.Tasks[0];

		viewModel.DeleteCommand.Execute(null);
		await Task.Delay(50);

		Assert.Empty(viewModel.Tasks);
		Assert.Null(viewModel.SelectedTask);
		Assert.Equal(1, fake.LastDeletedId);
	}
}
