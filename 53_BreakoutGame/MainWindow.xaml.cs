using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using BreakoutGame.Engine;
using BreakoutGame.Models;

namespace BreakoutGame;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	private static readonly Brush[] RowColors =
	[
		Brushes.OrangeRed, Brushes.Gold, Brushes.LimeGreen, Brushes.DodgerBlue,
	];

	private GameEngine? _engine;
	private readonly Dictionary<Block, Rectangle> _blockShapes = [];
	private Rectangle? _paddleShape;
	private Ellipse? _ballShape;
	private readonly Stopwatch _stopwatch = new();

	public MainWindow()
	{
		InitializeComponent();
		Loaded += MainWindow_Loaded;
	}

	private void MainWindow_Loaded(object sender, RoutedEventArgs e)
	{
		StartNewGame();
		_stopwatch.Start();
		CompositionTarget.Rendering += CompositionTarget_Rendering;
	}

	private void StartNewGame()
	{
		GameCanvas.Children.Clear();
		_blockShapes.Clear();

		var width = GameCanvas.ActualWidth > 0 ? GameCanvas.ActualWidth : 400;
		var height = GameCanvas.ActualHeight > 0 ? GameCanvas.ActualHeight : 500;
		_engine = new GameEngine(width, height);

		for (var i = 0; i < _engine.Blocks.Count; i++)
		{
			var block = _engine.Blocks[i];
			var row = i / 8;
			var rect = new Rectangle
			{
				Width = block.Bounds.Width,
				Height = block.Bounds.Height,
				Fill = RowColors[row % RowColors.Length],
			};
			Canvas.SetLeft(rect, block.Bounds.Left);
			Canvas.SetTop(rect, block.Bounds.Top);
			GameCanvas.Children.Add(rect);
			_blockShapes[block] = rect;
		}

		_paddleShape = new Rectangle { Width = _engine.Paddle.Width, Height = _engine.Paddle.Height, Fill = Brushes.White };
		GameCanvas.Children.Add(_paddleShape);

		_ballShape = new Ellipse
		{
			Width = _engine.Ball.Radius * 2,
			Height = _engine.Ball.Radius * 2,
			Fill = Brushes.Yellow,
		};
		GameCanvas.Children.Add(_ballShape);
	}

	private void CompositionTarget_Rendering(object? sender, EventArgs e)
	{
		if (_engine is null)
		{
			return;
		}

		var elapsed = _stopwatch.Elapsed.TotalSeconds;
		_stopwatch.Restart();
		// タブ切り替え等で極端に大きいdeltaが発生した場合の暴走を防ぐ
		elapsed = Math.Min(elapsed, 0.1);

		_engine.Update(elapsed);
		Render();
	}

	private void Render()
	{
		if (_engine is null || _paddleShape is null || _ballShape is null)
		{
			return;
		}

		foreach (var (block, rect) in _blockShapes)
		{
			rect.Visibility = block.IsDestroyed ? Visibility.Collapsed : Visibility.Visible;
		}

		Canvas.SetLeft(_paddleShape, _engine.Paddle.Position.X);
		Canvas.SetTop(_paddleShape, _engine.Paddle.Position.Y);

		Canvas.SetLeft(_ballShape, _engine.Ball.Position.X - _engine.Ball.Radius);
		Canvas.SetTop(_ballShape, _engine.Ball.Position.Y - _engine.Ball.Radius);

		ScoreText.Text = _engine.Score.ToString();
		LivesText.Text = _engine.Lives.ToString();
		StatusText.Text = _engine.Status switch
		{
			GameStatus.Cleared => "クリア!",
			GameStatus.GameOver => "ゲームオーバー",
			_ => string.Empty,
		};
	}

	private void Window_KeyDown(object sender, KeyEventArgs e)
	{
		switch (e.Key)
		{
			case Key.Left:
				_engine?.SetPaddleDirection(-1);
				break;
			case Key.Right:
				_engine?.SetPaddleDirection(1);
				break;
		}
	}

	private void Window_KeyUp(object sender, KeyEventArgs e)
	{
		if (e.Key is Key.Left or Key.Right)
		{
			_engine?.SetPaddleDirection(0);
		}
	}

	private void RestartButton_Click(object sender, RoutedEventArgs e)
	{
		_engine?.Restart();
		Render();
	}
}
