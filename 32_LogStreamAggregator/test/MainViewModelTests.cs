using LogStreamAggregator.Models;
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

	/// <summary>
	/// パス条件: Producerで予期しない例外が発生してもクラッシュせず、ErrorMessageが設定され
	/// IsRunningがfalseに戻ること(fire-and-forgetタスクの未処理例外対策の確認)。
	/// </summary>
	[Fact]
	public async Task StartCommand_Producerが例外を投げてもクラッシュせずErrorMessageが設定される()
	{
		var callCount = 0;
		LogEntry ThrowingGenerator()
		{
			callCount++;
			if (callCount >= 2)
			{
				throw new InvalidOperationException("ログ生成に失敗しました");
			}

			return new LogEntry(DateTime.Now, LogLevel.Info, "ok");
		}

		var viewModel = new MainViewModel(new ImmediateUiDispatcher(), ThrowingGenerator);

		viewModel.StartCommand.Execute(null);
		var waited = 0;
		while (viewModel.IsRunning && waited < 3000)
		{
			await Task.Delay(50);
			waited += 50;
		}

		Assert.False(viewModel.IsRunning);
		Assert.False(string.IsNullOrEmpty(viewModel.ErrorMessage));
	}
}
