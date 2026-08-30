using GameOfLife.ViewModels;

namespace GameOfLife.Tests;

/// <summary>
/// <see cref="MainViewModel"/> の単体テスト。
/// </summary>
public class MainViewModelTests
{
	/// <summary>
	/// パス条件: PlayPauseCommand実行でIsRunningが反転すること
	/// </summary>
	[Fact]
	public void PlayPauseCommand_実行するとIsRunningが反転する()
	{
		var viewModel = new MainViewModel(10, 10);

		viewModel.PlayPauseCommand.Execute(null);
		Assert.True(viewModel.IsRunning);

		viewModel.PlayPauseCommand.Execute(null);
		Assert.False(viewModel.IsRunning);
	}

	/// <summary>
	/// パス条件: AdvanceGeneration呼び出しでGenerationがインクリメントされ盤面が1世代進むこと
	/// </summary>
	[Fact]
	public void AdvanceGeneration_呼び出すとGenerationがインクリメントされ盤面が進む()
	{
		var viewModel = new MainViewModel(3, 3);
		viewModel.Engine.SetAlive(0, 0, true);
		viewModel.Engine.SetAlive(1, 0, true);
		viewModel.Engine.SetAlive(0, 1, true);

		viewModel.AdvanceGeneration();

		Assert.Equal(1, viewModel.Generation);
		Assert.True(viewModel.Engine.IsAlive(1, 1));
	}

	/// <summary>
	/// パス条件: ResetCommand実行でEngineがクリアされ、Generationが0・IsRunningがfalseに戻ること
	/// </summary>
	[Fact]
	public void ResetCommand_実行すると盤面と状態が初期化される()
	{
		var viewModel = new MainViewModel(3, 3);
		viewModel.Engine.SetAlive(1, 1, true);
		viewModel.AdvanceGeneration();
		viewModel.PlayPauseCommand.Execute(null);

		viewModel.ResetCommand.Execute(null);

		Assert.False(viewModel.Engine.IsAlive(1, 1));
		Assert.Equal(0, viewModel.Generation);
		Assert.False(viewModel.IsRunning);
	}

	/// <summary>
	/// パス条件: AdvanceGeneration実行でBoardChangedイベントが発火すること(Viewの再描画契機)
	/// </summary>
	[Fact]
	public void AdvanceGeneration_実行するとBoardChangedが発火する()
	{
		var viewModel = new MainViewModel(3, 3);
		var raised = false;
		viewModel.BoardChanged += (_, _) => raised = true;

		viewModel.AdvanceGeneration();

		Assert.True(raised);
	}

	/// <summary>
	/// パス条件: ResetCommand実行でBoardChangedイベントが発火すること(世代が0のままでも再描画が必要なため)
	/// </summary>
	[Fact]
	public void ResetCommand_実行するとGenerationが0のままでもBoardChangedが発火する()
	{
		var viewModel = new MainViewModel(3, 3);
		var raised = false;
		viewModel.BoardChanged += (_, _) => raised = true;

		viewModel.ResetCommand.Execute(null);

		Assert.True(raised);
	}

	/// <summary>
	/// パス条件: IntervalMillisecondsは許容範囲(50〜2000ミリ秒)にクランプされること
	/// </summary>
	[Theory]
	[InlineData(0, 50)]
	[InlineData(5000, 2000)]
	[InlineData(500, 500)]
	public void IntervalMilliseconds_許容範囲にクランプされる(int input, int expected)
	{
		var viewModel = new MainViewModel(3, 3);

		viewModel.IntervalMilliseconds = input;

		Assert.Equal(expected, viewModel.IntervalMilliseconds);
	}
}
