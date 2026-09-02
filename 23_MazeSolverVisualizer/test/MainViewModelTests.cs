using MazeSolverVisualizer.Services;
using MazeSolverVisualizer.ViewModels;

namespace MazeSolverVisualizer.Tests;

/// <summary>
/// <see cref="MainViewModel"/> の単体テスト。生成・探索は実際の
/// <see cref="RecursiveBacktrackerMazeGenerator"/>/<see cref="BfsMazeSolver"/>等をシード固定で使う
/// (いずれも高速・決定的な純粋ロジックのためフェイク不要)。
/// </summary>
public class MainViewModelTests
{
	private static MainViewModel CreateViewModel() =>
		new(new RecursiveBacktrackerMazeGenerator(), [new BfsMazeSolver(), new DfsMazeSolver()], new Random(1));

	/// <summary>
	/// パス条件: コンストラクタ実行時に迷路が生成されること
	/// </summary>
	[Fact]
	public void コンストラクタ_初期化時に迷路が生成される()
	{
		var viewModel = CreateViewModel();

		Assert.NotNull(viewModel.Maze);
	}

	/// <summary>
	/// パス条件: GenerateCommand実行で迷路が再生成されMazeChangedが発火すること
	/// </summary>
	[Fact]
	public void GenerateCommand_実行すると迷路が再生成されMazeChangedが発火する()
	{
		var viewModel = CreateViewModel();
		var raised = false;
		viewModel.MazeChanged += (_, _) => raised = true;

		viewModel.GenerateCommand.Execute(null);

		Assert.True(raised);
	}

	/// <summary>
	/// パス条件: SolveCommand実行で選択中アルゴリズムの探索結果がLastResultに設定されること
	/// </summary>
	[Fact]
	public void SolveCommand_実行すると探索結果が設定される()
	{
		var viewModel = CreateViewModel();

		viewModel.SolveCommand.Execute(null);

		Assert.NotNull(viewModel.LastResult);
		Assert.NotNull(viewModel.LastResult!.Path);
		Assert.Equal(MainViewModel.Start, viewModel.LastResult.Path![0]);
		Assert.Equal(MainViewModel.Goal, viewModel.LastResult.Path[^1]);
	}

	/// <summary>
	/// パス条件: SolveCommand実行後、AnimationStepIndexが0にリセットされること
	/// </summary>
	[Fact]
	public void SolveCommand_実行後AnimationStepIndexが0になる()
	{
		var viewModel = CreateViewModel();

		viewModel.SolveCommand.Execute(null);

		Assert.Equal(0, viewModel.AnimationStepIndex);
	}

	/// <summary>
	/// パス条件: AdvanceAnimationStepでAnimationStepIndexが1増えること
	/// </summary>
	[Fact]
	public void AdvanceAnimationStep_呼び出すとAnimationStepIndexが1増える()
	{
		var viewModel = CreateViewModel();
		viewModel.SolveCommand.Execute(null);

		viewModel.AdvanceAnimationStep();

		Assert.Equal(1, viewModel.AnimationStepIndex);
	}

	/// <summary>
	/// パス条件: AdvanceAnimationStepはVisitedOrderの件数を超えて進まないこと
	/// </summary>
	[Fact]
	public void AdvanceAnimationStep_訪問セル数を超えて進まない()
	{
		var viewModel = CreateViewModel();
		viewModel.SolveCommand.Execute(null);
		var totalSteps = viewModel.LastResult!.VisitedOrder.Count;

		for (var i = 0; i < totalSteps + 10; i++)
		{
			viewModel.AdvanceAnimationStep();
		}

		Assert.Equal(totalSteps, viewModel.AnimationStepIndex);
	}

	/// <summary>
	/// パス条件: 最終ステップに到達するとIsAnimationCompleteがtrueになること
	/// </summary>
	[Fact]
	public void IsAnimationComplete_最終ステップでtrueになる()
	{
		var viewModel = CreateViewModel();
		viewModel.SolveCommand.Execute(null);
		var totalSteps = viewModel.LastResult!.VisitedOrder.Count;

		Assert.False(viewModel.IsAnimationComplete);
		for (var i = 0; i < totalSteps; i++)
		{
			viewModel.AdvanceAnimationStep();
		}

		Assert.True(viewModel.IsAnimationComplete);
	}

	/// <summary>
	/// パス条件: 新しい迷路を生成すると、直前の探索結果(LastResult)がクリアされること
	/// </summary>
	[Fact]
	public void GenerateCommand_実行すると直前の探索結果がクリアされる()
	{
		var viewModel = CreateViewModel();
		viewModel.SolveCommand.Execute(null);

		viewModel.GenerateCommand.Execute(null);

		Assert.Null(viewModel.LastResult);
	}
}
