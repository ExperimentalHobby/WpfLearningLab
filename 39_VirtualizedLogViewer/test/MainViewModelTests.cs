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

	/// <summary>
	/// パス条件: GenerateCommand実行で、指定件数のログが生成されDisplayedLinesに反映されること
	/// (ファイル書き出し→ストリーミング読み込みという実装に変更しても、外部から見た内容は変わらない)。
	/// </summary>
	[Fact]
	public async Task GenerateCommand_実行すると指定件数のログが生成される()
	{
		var viewModel = new MainViewModel { LineCountInput = "10" };

		viewModel.GenerateCommand.Execute(null);
		await Task.Delay(500);

		Assert.Equal(10, viewModel.DisplayedLines.Count);
		Assert.Equal(1, viewModel.DisplayedLines[0].LineNumber);
		Assert.Equal("Sample log message number 1", viewModel.DisplayedLines[0].Message);
	}

	/// <summary>
	/// パス条件: GenerateCommand実行で、DisplayedLinesが(1件ずつAddするのではなく)
	/// コレクションごと新しいインスタンスに差し替えられること。
	/// 大量件数(10万件)で1件ずつAddするとCollectionChangedが同数発火しUIが固まっていた
	/// 実バグの回帰テスト。
	/// </summary>
	[Fact]
	public async Task GenerateCommand_実行するとDisplayedLinesがコレクションごと差し替えられる()
	{
		var viewModel = new MainViewModel { LineCountInput = "10" };
		var originalCollection = viewModel.DisplayedLines;

		viewModel.GenerateCommand.Execute(null);
		await Task.Delay(500);

		Assert.Equal(10, viewModel.DisplayedLines.Count);
		Assert.NotSame(originalCollection, viewModel.DisplayedLines);
	}
}
