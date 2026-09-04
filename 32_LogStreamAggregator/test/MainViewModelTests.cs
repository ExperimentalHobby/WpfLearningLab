using LogStreamAggregator.Tests.Fakes;
using LogStreamAggregator.ViewModels;

namespace LogStreamAggregator.Tests;

/// <summary>
/// <see cref="MainViewModel"/>のテスト。実の非同期Producer/Consumerタスクを短時間実行して検証する。
/// </summary>
public class MainViewModelTests
{
	/// <summary>
	/// パス条件: 開始すると実際にログが生成・集計され、TotalCount/RecentLogsが増加すること。
	/// </summary>
	[Fact]
	public async Task StartCommand_実行するとログが集計されTotalCountが増加する()
	{
		var viewModel = new MainViewModel(new ImmediateUiDispatcher());

		viewModel.StartCommand.Execute(null);
		await Task.Delay(600);
		viewModel.StopCommand.Execute(null);

		Assert.True(viewModel.TotalCount > 0);
		Assert.NotEmpty(viewModel.RecentLogs);
		Assert.False(viewModel.IsRunning);
	}

	/// <summary>
	/// パス条件: 開始直後はStartCommandが実行不可、StopCommandが実行可能になること。
	/// </summary>
	[Fact]
	public void StartCommand_実行するとStart不可Stop可能になる()
	{
		var viewModel = new MainViewModel(new ImmediateUiDispatcher());

		viewModel.StartCommand.Execute(null);

		Assert.False(viewModel.StartCommand.CanExecute(null));
		Assert.True(viewModel.StopCommand.CanExecute(null));

		viewModel.StopCommand.Execute(null);
	}
}
