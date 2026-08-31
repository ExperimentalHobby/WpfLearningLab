using VirtualizedLogViewer.ViewModels;

namespace VirtualizedLogViewer.Tests;

/// <summary>
/// <see cref="MainViewModel"/>のテスト。
/// </summary>
public class MainViewModelTests
{
	/// <summary>
	/// パス条件: GenerateCommand実行後、JumpToLineCommandが実行可能になること。
	/// (DisplayedLines更新時にJumpToLineCommandのCanExecute再評価を呼び忘れると、
	/// 実機ではUI Automationの自動再評価タイミングに依存して無効なままになる実バグの回帰テスト)
	/// </summary>
	[Fact]
	public async Task GenerateCommand_実行後JumpToLineCommandが実行可能になる()
	{
		var viewModel = new MainViewModel { LineCountInput = "10" };

		viewModel.GenerateCommand.Execute(null);
		await Task.Delay(500);

		Assert.True(viewModel.JumpToLineCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: GenerateCommand実行前は、JumpToLineCommandが実行不可であること。
	/// </summary>
	[Fact]
	public void JumpToLineCommand_生成前は実行不可()
	{
		var viewModel = new MainViewModel();

		Assert.False(viewModel.JumpToLineCommand.CanExecute(null));
	}
}
