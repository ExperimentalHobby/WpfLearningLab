using MazeSolverVisualizer.Models;
using MazeSolverVisualizer.Services;

namespace MazeSolverVisualizer.ViewModels;

/// <summary>
/// 迷路生成&探索ビジュアライザのメイン画面のViewModel。迷路の生成・探索実行・
/// アニメーション再生位置(ステップindex)を管理する。実際の描画はView層が担う。
/// </summary>
public class MainViewModel : ObservableObject
{
	/// <summary>迷路の幅(セル数)。</summary>
	public const int Width = 15;

	/// <summary>迷路の高さ(セル数)。</summary>
	public const int Height = 15;

	/// <summary>スタート地点。</summary>
	public static readonly (int X, int Y) Start = (0, 0);

	/// <summary>ゴール地点。</summary>
	public static readonly (int X, int Y) Goal = (Width - 1, Height - 1);

	private readonly IMazeGenerator _generator;
	private readonly Random _random;

	private MazeSolverResult? _lastResult;
	private int _animationStepIndex;
	private IMazeSolver _selectedSolver;

	/// <summary>
	/// ViewModelを初期化し、最初の迷路を生成する。
	/// </summary>
	/// <param name="generator">迷路生成を担うジェネレーター。</param>
	/// <param name="solvers">選択可能な探索アルゴリズムの一覧。</param>
	/// <param name="random">迷路生成に使う乱数生成器。テストではシード固定の<see cref="Random"/>を渡す。</param>
	public MainViewModel(IMazeGenerator generator, IReadOnlyList<IMazeSolver> solvers, Random random)
	{
		_generator = generator;
		Solvers = solvers;
		_random = random;
		_selectedSolver = solvers[0];

		GenerateCommand = new RelayCommand(Generate);
		SolveCommand = new RelayCommand(Solve);

		Generate();
	}

	/// <summary>現在の迷路。</summary>
	public Maze Maze { get; private set; } = null!;

	/// <summary>選択可能な探索アルゴリズムの一覧。</summary>
	public IReadOnlyList<IMazeSolver> Solvers { get; }

	/// <summary>選択中の探索アルゴリズム。</summary>
	public IMazeSolver SelectedSolver
	{
		get => _selectedSolver;
		set => SetProperty(ref _selectedSolver, value);
	}

	/// <summary>直近の探索結果。未探索の場合は<see langword="null"/>。</summary>
	public MazeSolverResult? LastResult
	{
		get => _lastResult;
		private set => SetProperty(ref _lastResult, value);
	}

	/// <summary>アニメーションの現在の再生位置(訪問済みとして表示するセル数)。</summary>
	public int AnimationStepIndex
	{
		get => _animationStepIndex;
		private set => SetProperty(ref _animationStepIndex, value);
	}

	/// <summary>アニメーションが最後まで再生し終えたかどうか。</summary>
	public bool IsAnimationComplete => LastResult is not null && AnimationStepIndex >= LastResult.VisitedOrder.Count;

	/// <summary>迷路が新しく生成されたときに発火する(View層の再描画契機)。</summary>
	public event EventHandler? MazeChanged;

	/// <summary>新しい迷路を生成するコマンド。</summary>
	public RelayCommand GenerateCommand { get; }

	/// <summary>選択中のアルゴリズムで探索を実行するコマンド。</summary>
	public RelayCommand SolveCommand { get; }

	/// <summary>
	/// アニメーションを1ステップ進める。<see cref="System.Windows.Threading.DispatcherTimer"/>のTickからView層が呼び出す。
	/// </summary>
	public void AdvanceAnimationStep()
	{
		if (IsAnimationComplete)
		{
			return;
		}

		AnimationStepIndex++;
	}

	private void Generate()
	{
		Maze = _generator.Generate(Width, Height, _random);
		LastResult = null;
		AnimationStepIndex = 0;
		MazeChanged?.Invoke(this, EventArgs.Empty);
	}

	private void Solve()
	{
		LastResult = SelectedSolver.Solve(Maze, Start, Goal);
		AnimationStepIndex = 0;
	}
}
