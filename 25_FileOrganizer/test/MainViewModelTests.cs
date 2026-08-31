using FileOrganizer.Models;
using FileOrganizer.ViewModels;

namespace FileOrganizer.Tests;

/// <summary>
/// <see cref="MainViewModel"/> の単体テスト。
/// </summary>
public class MainViewModelTests
{
	private static MainViewModel CreateViewModel(
		FakeFileOrganizerService? organizerService = null,
		FakeDirectoryWatcher? watcher = null,
		FakeFolderPicker? folderPicker = null,
		FakeUiDispatcher? dispatcher = null) =>
		new(
			organizerService ?? new FakeFileOrganizerService(),
			watcher ?? new FakeDirectoryWatcher(),
			folderPicker ?? new FakeFolderPicker(),
			dispatcher ?? new FakeUiDispatcher());

	/// <summary>
	/// パス条件: SelectFolderCommand実行でpickerが返したフォルダがWatchFolderに設定されること
	/// </summary>
	[Fact]
	public void SelectFolderCommand_実行するとpickerが返したフォルダがWatchFolderに設定される()
	{
		var picker = new FakeFolderPicker { FolderToReturn = @"C:\Downloads" };
		var viewModel = CreateViewModel(folderPicker: picker);

		viewModel.SelectFolderCommand.Execute(null);

		Assert.Equal(@"C:\Downloads", viewModel.WatchFolder);
	}

	/// <summary>
	/// パス条件: SelectFolderCommand実行時にpickerがnullを返す(キャンセル)場合、WatchFolderは変化しないこと
	/// </summary>
	[Fact]
	public void SelectFolderCommand_pickerがnullを返す場合WatchFolderは変化しない()
	{
		var picker = new FakeFolderPicker { FolderToReturn = null };
		var viewModel = CreateViewModel(folderPicker: picker);

		viewModel.SelectFolderCommand.Execute(null);

		Assert.Equal(string.Empty, viewModel.WatchFolder);
	}

	/// <summary>
	/// パス条件: AddRuleCommand実行でRulesにルールが追加されること(拡張子の先頭に"."が無ければ補完される)
	/// </summary>
	[Fact]
	public void AddRuleCommand_実行するとRulesにルールが追加され拡張子の先頭のドットが補完される()
	{
		var viewModel = CreateViewModel();
		viewModel.NewRuleExtension = "jpg";
		viewModel.NewRuleDestination = "Images";

		viewModel.AddRuleCommand.Execute(null);

		Assert.Single(viewModel.Rules);
		Assert.Equal(new SortingRule(".jpg", "Images"), viewModel.Rules[0]);
	}

	/// <summary>
	/// パス条件: AddRuleCommand実行後、入力欄がクリアされること
	/// </summary>
	[Fact]
	public void AddRuleCommand_実行すると入力欄がクリアされる()
	{
		var viewModel = CreateViewModel();
		viewModel.NewRuleExtension = ".jpg";
		viewModel.NewRuleDestination = "Images";

		viewModel.AddRuleCommand.Execute(null);

		Assert.Equal(string.Empty, viewModel.NewRuleExtension);
		Assert.Equal(string.Empty, viewModel.NewRuleDestination);
	}

	/// <summary>
	/// パス条件: 拡張子または移動先フォルダ名の入力欄が空欄の場合、AddRuleCommandが実行不可になること
	/// </summary>
	[Theory]
	[InlineData("", "Images")]
	[InlineData(".jpg", "")]
	[InlineData("", "")]
	public void AddRuleCommand_入力欄が空欄の場合CanExecuteがfalseになる(string extension, string destination)
	{
		var viewModel = CreateViewModel();
		viewModel.NewRuleExtension = extension;
		viewModel.NewRuleDestination = destination;

		Assert.False(viewModel.AddRuleCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: RemoveRuleCommand実行でRulesから指定したルールが削除されること
	/// </summary>
	[Fact]
	public void RemoveRuleCommand_実行するとRulesから指定したルールが削除される()
	{
		var viewModel = CreateViewModel();
		viewModel.NewRuleExtension = ".jpg";
		viewModel.NewRuleDestination = "Images";
		viewModel.AddRuleCommand.Execute(null);
		var rule = viewModel.Rules[0];

		viewModel.RemoveRuleCommand.Execute(rule);

		Assert.Empty(viewModel.Rules);
	}

	/// <summary>
	/// パス条件: StartWatchingCommand実行でwatcherが開始されIsWatchingがtrueになること
	/// </summary>
	[Fact]
	public void StartWatchingCommand_実行するとwatcherが開始されIsWatchingがtrueになる()
	{
		var watcher = new FakeDirectoryWatcher();
		var picker = new FakeFolderPicker { FolderToReturn = @"C:\Downloads" };
		var viewModel = CreateViewModel(watcher: watcher, folderPicker: picker);
		viewModel.SelectFolderCommand.Execute(null);

		viewModel.StartWatchingCommand.Execute(null);

		Assert.True(viewModel.IsWatching);
		Assert.True(watcher.IsStarted);
		Assert.Equal(@"C:\Downloads", watcher.StartedFolder);
	}

	/// <summary>
	/// パス条件: StopWatchingCommand実行でwatcherが停止しIsWatchingがfalseになること
	/// </summary>
	[Fact]
	public void StopWatchingCommand_実行するとwatcherが停止しIsWatchingがfalseになる()
	{
		var watcher = new FakeDirectoryWatcher();
		var picker = new FakeFolderPicker { FolderToReturn = @"C:\Downloads" };
		var viewModel = CreateViewModel(watcher: watcher, folderPicker: picker);
		viewModel.SelectFolderCommand.Execute(null);
		viewModel.StartWatchingCommand.Execute(null);

		viewModel.StopWatchingCommand.Execute(null);

		Assert.False(viewModel.IsWatching);
		Assert.False(watcher.IsStarted);
	}

	/// <summary>
	/// パス条件: WatchFolderが未設定の場合、StartWatchingCommandが実行不可になること
	/// </summary>
	[Fact]
	public void StartWatchingCommand_WatchFolder未設定の場合CanExecuteがfalseになる()
	{
		var viewModel = CreateViewModel();

		Assert.False(viewModel.StartWatchingCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: 監視中の場合、StartWatchingCommandが実行不可になりStopWatchingCommandが実行可能になること
	/// </summary>
	[Fact]
	public void StartWatchingCommand_監視中の場合CanExecuteがfalseになりStopWatchingCommandが実行可能になる()
	{
		var picker = new FakeFolderPicker { FolderToReturn = @"C:\Downloads" };
		var viewModel = CreateViewModel(folderPicker: picker);
		viewModel.SelectFolderCommand.Execute(null);
		viewModel.StartWatchingCommand.Execute(null);

		Assert.False(viewModel.StartWatchingCommand.CanExecute(null));
		Assert.True(viewModel.StopWatchingCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: OrganizeExistingCommand実行で一括整理され、移動できた結果がLogsに記録されること
	/// </summary>
	[Fact]
	public async Task OrganizeExistingCommand_実行すると一括整理されLogsに記録される()
	{
		var organizerService = new FakeFileOrganizerService
		{
			ExistingFilesResult =
			[
				new OrganizeResult(@"C:\Downloads\a.jpg", @"C:\Downloads\Images\a.jpg", true, DateTime.Now),
			],
		};
		var picker = new FakeFolderPicker { FolderToReturn = @"C:\Downloads" };
		var viewModel = CreateViewModel(organizerService: organizerService, folderPicker: picker);
		viewModel.SelectFolderCommand.Execute(null);

		viewModel.OrganizeExistingCommand.Execute(null);
		await Task.Delay(50);

		Assert.Single(viewModel.Logs);
		Assert.Equal(@"C:\Downloads", organizerService.RequestedWatchFolder);
	}

	/// <summary>
	/// パス条件: OrganizeExistingCommand実行で移動されなかった結果はLogsに追加されないこと
	/// </summary>
	[Fact]
	public async Task OrganizeExistingCommand_移動されなかった結果はLogsに追加されない()
	{
		var organizerService = new FakeFileOrganizerService
		{
			ExistingFilesResult =
			[
				new OrganizeResult(@"C:\Downloads\readme.txt", null, false, DateTime.Now),
			],
		};
		var picker = new FakeFolderPicker { FolderToReturn = @"C:\Downloads" };
		var viewModel = CreateViewModel(organizerService: organizerService, folderPicker: picker);
		viewModel.SelectFolderCommand.Execute(null);

		viewModel.OrganizeExistingCommand.Execute(null);
		await Task.Delay(50);

		Assert.Empty(viewModel.Logs);
	}

	/// <summary>
	/// パス条件: WatchFolderが未設定の場合、OrganizeExistingCommandが実行不可になること
	/// </summary>
	[Fact]
	public void OrganizeExistingCommand_WatchFolder未設定の場合CanExecuteがfalseになる()
	{
		var viewModel = CreateViewModel();

		Assert.False(viewModel.OrganizeExistingCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: watcherのFileCreated発火で自動的に振り分けられLogsに追加されること(フェイクDispatcher経由)
	/// </summary>
	[Fact]
	public async Task FileCreated_発火すると自動的に振り分けられLogsに追加される()
	{
		var organizerService = new FakeFileOrganizerService
		{
			OrganizeFileResultFactory = (path, _, _) =>
				new OrganizeResult(path, @"C:\Downloads\Images\new.jpg", true, DateTime.Now),
		};
		var watcher = new FakeDirectoryWatcher();
		var picker = new FakeFolderPicker { FolderToReturn = @"C:\Downloads" };
		var dispatcher = new FakeUiDispatcher();
		var viewModel = CreateViewModel(organizerService, watcher, picker, dispatcher);
		viewModel.SelectFolderCommand.Execute(null);
		viewModel.StartWatchingCommand.Execute(null);

		watcher.RaiseFileCreated(@"C:\Downloads\new.jpg");
		await Task.Delay(50);

		Assert.Single(viewModel.Logs);
	}

	/// <summary>
	/// パス条件: watcherのFileCreated発火時にルールに合致しない場合、Logsに追加されないこと
	/// </summary>
	[Fact]
	public async Task FileCreated_ルールに合致しない場合Logsに追加されない()
	{
		var organizerService = new FakeFileOrganizerService
		{
			OrganizeFileResultFactory = (path, _, _) => new OrganizeResult(path, null, false, DateTime.Now),
		};
		var watcher = new FakeDirectoryWatcher();
		var picker = new FakeFolderPicker { FolderToReturn = @"C:\Downloads" };
		var viewModel = CreateViewModel(organizerService, watcher, picker);
		viewModel.SelectFolderCommand.Execute(null);
		viewModel.StartWatchingCommand.Execute(null);

		watcher.RaiseFileCreated(@"C:\Downloads\readme.txt");
		await Task.Delay(50);

		Assert.Empty(viewModel.Logs);
	}
}
