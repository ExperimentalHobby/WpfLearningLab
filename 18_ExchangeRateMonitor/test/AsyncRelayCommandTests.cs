using ExchangeRateMonitor.ViewModels;

namespace ExchangeRateMonitor.Tests;

/// <summary>
/// <see cref="AsyncRelayCommand"/> の単体テスト。
/// </summary>
public class AsyncRelayCommandTests
{
	/// <summary>
	/// パス条件: 実行中は多重実行を防ぐためCanExecuteがfalseになること
	/// </summary>
	[Fact]
	public async Task CanExecute_実行中はfalseになる()
	{
		var tcs = new TaskCompletionSource();
		var command = new AsyncRelayCommand(() => tcs.Task);

		command.Execute(null);
		var canExecuteDuringRun = command.CanExecute(null);

		tcs.SetResult();
		await Task.Delay(10);

		Assert.False(canExecuteDuringRun);
	}

	/// <summary>
	/// パス条件: 実行完了後はCanExecuteがtrueに戻ること
	/// </summary>
	[Fact]
	public async Task CanExecute_実行完了後にtrueへ戻る()
	{
		var tcs = new TaskCompletionSource();
		var command = new AsyncRelayCommand(() => tcs.Task);

		command.Execute(null);
		tcs.SetResult();
		await Task.Delay(10);

		Assert.True(command.CanExecute(null));
	}

	/// <summary>
	/// パス条件: 外部canExecuteデリゲートがfalseを返す場合、CanExecuteがfalseになること
	/// </summary>
	[Fact]
	public void CanExecute_外部条件がfalseの場合falseになる()
	{
		var command = new AsyncRelayCommand(() => Task.CompletedTask, () => false);

		var result = command.CanExecute(null);

		Assert.False(result);
	}

	/// <summary>
	/// パス条件: 実行中に(CanExecuteを確認せず)再度Executeを呼んでも、多重実行されないこと
	/// (DispatcherTimer.Tickから直接Execute(null)を呼ぶような使い方でも、
	/// 前回の実行が完了していなければ多重実行してはならないため)
	/// </summary>
	[Fact]
	public async Task Execute_実行中に再度Executeを呼んでも多重実行されない()
	{
		var executionCount = 0;
		var tcs = new TaskCompletionSource();
		var command = new AsyncRelayCommand(async () =>
		{
			executionCount++;
			await tcs.Task;
		});

		command.Execute(null);
		command.Execute(null);

		tcs.SetResult();
		await Task.Delay(10);

		Assert.Equal(1, executionCount);
	}
}
