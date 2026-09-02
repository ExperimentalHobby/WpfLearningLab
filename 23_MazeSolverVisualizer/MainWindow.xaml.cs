using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using MazeSolverVisualizer.Services;
using MazeSolverVisualizer.ViewModels;

namespace MazeSolverVisualizer;

/// <summary>
/// 迷路生成&amp;探索ビジュアライザのメイン画面。迷路の描画(壁・セルの色分け)と
/// <see cref="DispatcherTimer"/>によるアニメーション再生はView固有の関心事としてこのコードビハインドが担う。
/// </summary>
public partial class MainWindow : Window
{
	private const double CellSize = 24;

	private readonly MainViewModel _viewModel;
	private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(60) };
	private Rectangle[,] _cellRects = new Rectangle[0, 0];

	public MainWindow()
	{
		InitializeComponent();

		_viewModel = new MainViewModel(
			new RecursiveBacktrackerMazeGenerator(),
			[new BfsMazeSolver(), new DfsMazeSolver(), new DijkstraMazeSolver(), new AStarMazeSolver()],
			new Random());
		DataContext = _viewModel;

		DrawMaze();

		_timer.Tick += (_, _) =>
		{
			_viewModel.AdvanceAnimationStep();
			UpdateCellColors();
			if (_viewModel.IsAnimationComplete)
			{
				_timer.Stop();
			}
		};

		_viewModel.MazeChanged += (_, _) => DrawMaze();
		_viewModel.PropertyChanged += OnViewModelPropertyChanged;
	}

	private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(MainViewModel.LastResult) && _viewModel.LastResult is not null)
		{
			UpdateCellColors();
			_timer.Start();
		}
	}

	private void DrawMaze()
	{
		_timer.Stop();
		GridCanvas.Children.Clear();

		var width = MainViewModel.Width;
		var height = MainViewModel.Height;
		GridCanvas.Width = width * CellSize;
		GridCanvas.Height = height * CellSize;
		_cellRects = new Rectangle[width, height];

		var maze = _viewModel.Maze;
		for (var x = 0; x < width; x++)
		{
			for (var y = 0; y < height; y++)
			{
				var rect = new Rectangle { Width = CellSize, Height = CellSize, Fill = Brushes.White };
				Canvas.SetLeft(rect, x * CellSize);
				Canvas.SetTop(rect, y * CellSize);
				GridCanvas.Children.Add(rect);
				_cellRects[x, y] = rect;

				if (y == 0 || !maze.IsConnected((x, y), (x, y - 1)))
				{
					AddWallLine(x * CellSize, y * CellSize, (x + 1) * CellSize, y * CellSize);
				}

				if (x == 0 || !maze.IsConnected((x, y), (x - 1, y)))
				{
					AddWallLine(x * CellSize, y * CellSize, x * CellSize, (y + 1) * CellSize);
				}

				if (y == height - 1)
				{
					AddWallLine(x * CellSize, (y + 1) * CellSize, (x + 1) * CellSize, (y + 1) * CellSize);
				}

				if (x == width - 1)
				{
					AddWallLine((x + 1) * CellSize, y * CellSize, (x + 1) * CellSize, (y + 1) * CellSize);
				}
			}
		}

		UpdateCellColors();
	}

	private void AddWallLine(double x1, double y1, double x2, double y2)
	{
		var line = new Line { X1 = x1, Y1 = y1, X2 = x2, Y2 = y2, Stroke = Brushes.Black, StrokeThickness = 2 };
		GridCanvas.Children.Add(line);
	}

	private void UpdateCellColors()
	{
		var width = MainViewModel.Width;
		var height = MainViewModel.Height;
		for (var x = 0; x < width; x++)
		{
			for (var y = 0; y < height; y++)
			{
				_cellRects[x, y].Fill = Brushes.White;
			}
		}

		var result = _viewModel.LastResult;
		if (result is not null)
		{
			foreach (var (x, y) in result.VisitedOrder.Take(_viewModel.AnimationStepIndex))
			{
				_cellRects[x, y].Fill = Brushes.LightBlue;
			}

			if (_viewModel.IsAnimationComplete && result.Path is not null)
			{
				foreach (var (x, y) in result.Path)
				{
					_cellRects[x, y].Fill = Brushes.Orange;
				}
			}
		}

		_cellRects[MainViewModel.Start.X, MainViewModel.Start.Y].Fill = Brushes.LimeGreen;
		_cellRects[MainViewModel.Goal.X, MainViewModel.Goal.Y].Fill = Brushes.Red;
	}
}
