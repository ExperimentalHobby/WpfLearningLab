using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using GameOfLife.ViewModels;

namespace GameOfLife;

/// <summary>
/// ライフゲームアプリのメイン画面。盤面の描画(Canvas+Rectangle群)とマウス操作によるセル配置、
/// <see cref="DispatcherTimer"/>による自動進行はView固有の関心事としてこのコードビハインドが担う。
/// ViewModelはEngineの状態と実行状態(再生/停止・世代・速度)のみを扱う。
/// </summary>
public partial class MainWindow : Window
{
	private const int GridWidth = 30;
	private const int GridHeight = 20;
	private const double CellSize = 20;

	private readonly MainViewModel _viewModel;
	private readonly DispatcherTimer _timer = new();
	private readonly Rectangle[,] _cells = new Rectangle[GridWidth, GridHeight];

	public MainWindow()
	{
		InitializeComponent();

		_viewModel = new MainViewModel(GridWidth, GridHeight);
		DataContext = _viewModel;

		BuildGrid();
		RedrawAll();

		_timer.Interval = TimeSpan.FromMilliseconds(_viewModel.IntervalMilliseconds);
		_timer.Tick += (_, _) =>
		{
			_viewModel.AdvanceGeneration();
			RedrawAll();
		};

		_viewModel.PropertyChanged += OnViewModelPropertyChanged;
		_viewModel.BoardChanged += (_, _) => RedrawAll();
	}

	private void BuildGrid()
	{
		GridCanvas.Width = GridWidth * CellSize;
		GridCanvas.Height = GridHeight * CellSize;

		for (var x = 0; x < GridWidth; x++)
		{
			for (var y = 0; y < GridHeight; y++)
			{
				var rect = new Rectangle
				{
					Width = CellSize,
					Height = CellSize,
					Stroke = Brushes.LightGray,
					StrokeThickness = 0.5,
				};
				Canvas.SetLeft(rect, x * CellSize);
				Canvas.SetTop(rect, y * CellSize);
				GridCanvas.Children.Add(rect);
				_cells[x, y] = rect;
			}
		}
	}

	private void RedrawAll()
	{
		for (var x = 0; x < GridWidth; x++)
		{
			for (var y = 0; y < GridHeight; y++)
			{
				_cells[x, y].Fill = _viewModel.Engine.IsAlive(x, y) ? Brushes.Black : Brushes.White;
			}
		}
	}

	private void GridCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		var position = e.GetPosition(GridCanvas);
		var x = (int)(position.X / CellSize);
		var y = (int)(position.Y / CellSize);
		if (x < 0 || x >= GridWidth || y < 0 || y >= GridHeight)
		{
			return;
		}

		_viewModel.Engine.ToggleCell(x, y);
		_cells[x, y].Fill = _viewModel.Engine.IsAlive(x, y) ? Brushes.Black : Brushes.White;
	}

	private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		switch (e.PropertyName)
		{
			case nameof(MainViewModel.IsRunning):
				if (_viewModel.IsRunning)
				{
					_timer.Start();
				}
				else
				{
					_timer.Stop();
				}

				break;
			case nameof(MainViewModel.IntervalMilliseconds):
				_timer.Interval = TimeSpan.FromMilliseconds(_viewModel.IntervalMilliseconds);
				break;
		}
	}
}
