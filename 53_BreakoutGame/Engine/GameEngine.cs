using System.Windows;
using BreakoutGame.Models;

namespace BreakoutGame.Engine;

/// <summary>
/// ボール・パドル・ブロックの状態を保持し、1フレーム分の状態遷移を行うUI非依存のゲームロジック。
/// </summary>
public class GameEngine
{
	private const double BallRadius = 8;
	private const double BallSpeed = 250;
	private const double PaddleWidth = 80;
	private const double PaddleHeight = 12;
	private const double PaddleSpeed = 400;
	private const int InitialLives = 3;
	private const int ScorePerBlock = 10;
	private const int Rows = 4;
	private const int Cols = 8;
	private const double BlockWidth = 40;
	private const double BlockHeight = 16;
	private const double BlockSpacing = 4;
	private const double BlockOffsetY = 40;

	private int _paddleDirection;
	private List<Block> _blocks = [];

	/// <summary>
	/// プレイフィールドの幅。
	/// </summary>
	public double FieldWidth { get; }

	/// <summary>
	/// プレイフィールドの高さ。
	/// </summary>
	public double FieldHeight { get; }

	/// <summary>
	/// ボールの現在状態。
	/// </summary>
	public Ball Ball { get; private set; }

	/// <summary>
	/// パドルの現在状態。
	/// </summary>
	public Paddle Paddle { get; private set; }

	/// <summary>
	/// ブロック一覧。
	/// </summary>
	public IReadOnlyList<Block> Blocks => _blocks;

	/// <summary>
	/// 現在のスコア。
	/// </summary>
	public int Score { get; private set; }

	/// <summary>
	/// 残りライフ。
	/// </summary>
	public int Lives { get; private set; }

	/// <summary>
	/// 現在のゲーム状態。
	/// </summary>
	public GameStatus Status { get; private set; }

	/// <summary>
	/// <see cref="GameEngine"/>を初期化する。
	/// </summary>
	/// <param name="fieldWidth">プレイフィールドの幅。</param>
	/// <param name="fieldHeight">プレイフィールドの高さ。</param>
	public GameEngine(double fieldWidth, double fieldHeight)
	{
		FieldWidth = fieldWidth;
		FieldHeight = fieldHeight;
		Paddle = CreateInitialPaddle();
		Ball = CreateInitialBall();
		Restart();
	}

	/// <summary>
	/// パドルの移動方向を設定する。
	/// </summary>
	/// <param name="direction">負値で左、正値で右、0で停止。</param>
	public void SetPaddleDirection(int direction) => _paddleDirection = Math.Sign(direction);

	/// <summary>
	/// スコア・ライフ・ブロック・ボール位置を初期状態に戻す。
	/// </summary>
	public void Restart()
	{
		Score = 0;
		Lives = InitialLives;
		Status = GameStatus.Playing;
		Paddle = CreateInitialPaddle();
		Ball = CreateInitialBall();
		_blocks = CreateBlocks();
	}

	/// <summary>
	/// テスト用: ボールの位置・速度を直接指定した状態に強制する。
	/// </summary>
	/// <param name="position">設定する位置。</param>
	/// <param name="velocity">設定する速度。</param>
	public void ForceBallState(Point position, Vector velocity) => Ball = Ball with { Position = position, Velocity = velocity };

	/// <summary>
	/// 1フレーム分の状態遷移を行う。
	/// </summary>
	/// <param name="deltaSeconds">経過時間(秒)。</param>
	public void Update(double deltaSeconds)
	{
		if (Status != GameStatus.Playing)
		{
			return;
		}

		UpdatePaddle(deltaSeconds);
		UpdateBall(deltaSeconds);

		if (_blocks.All(b => b.IsDestroyed))
		{
			Status = GameStatus.Cleared;
			return;
		}

		if (Ball.Position.Y - Ball.Radius > FieldHeight)
		{
			HandleBallFall();
		}
	}

	private void UpdatePaddle(double deltaSeconds)
	{
		var newX = Paddle.Position.X + (_paddleDirection * PaddleSpeed * deltaSeconds);
		newX = Math.Clamp(newX, 0, FieldWidth - Paddle.Width);
		Paddle = Paddle with { Position = new Point(newX, Paddle.Position.Y) };
	}

	private void UpdateBall(double deltaSeconds)
	{
		var newPosition = new Point(
			Ball.Position.X + (Ball.Velocity.X * deltaSeconds),
			Ball.Position.Y + (Ball.Velocity.Y * deltaSeconds));
		var velocity = Ball.Velocity;

		if (newPosition.X - Ball.Radius < 0)
		{
			newPosition.X = Ball.Radius;
			velocity.X = -velocity.X;
		}
		else if (newPosition.X + Ball.Radius > FieldWidth)
		{
			newPosition.X = FieldWidth - Ball.Radius;
			velocity.X = -velocity.X;
		}

		if (newPosition.Y - Ball.Radius < 0)
		{
			newPosition.Y = Ball.Radius;
			velocity.Y = -velocity.Y;
		}

		Ball = Ball with { Position = newPosition, Velocity = velocity };

		if (Ball.Velocity.Y > 0 && CollisionDetector.TryReflect(Ball, Paddle.Bounds, out _))
		{
			var reflected = CollisionDetector.ReflectOffPaddle(Ball, Paddle, BallSpeed);
			Ball = Ball with
			{
				Velocity = reflected,
				Position = new Point(Ball.Position.X, Paddle.Bounds.Top - Ball.Radius),
			};
		}

		foreach (var block in _blocks)
		{
			if (block.IsDestroyed)
			{
				continue;
			}

			if (CollisionDetector.TryReflect(Ball, block.Bounds, out var reflectedVelocity))
			{
				Ball = Ball with { Velocity = reflectedVelocity };
				block.IsDestroyed = true;
				Score += ScorePerBlock;
				break;
			}
		}
	}

	private void HandleBallFall()
	{
		Lives--;
		if (Lives <= 0)
		{
			Status = GameStatus.GameOver;
			return;
		}

		Ball = CreateInitialBall();
	}

	private Paddle CreateInitialPaddle() =>
		new(new Point((FieldWidth / 2) - (PaddleWidth / 2), FieldHeight - 40), PaddleWidth, PaddleHeight);

	private Ball CreateInitialBall() =>
		new(new Point(FieldWidth / 2, FieldHeight - 60), new Vector(BallSpeed * 0.5, -BallSpeed), BallRadius);

	private List<Block> CreateBlocks()
	{
		var blocks = new List<Block>();
		var totalWidth = (Cols * (BlockWidth + BlockSpacing)) - BlockSpacing;
		var offsetX = (FieldWidth - totalWidth) / 2;

		for (var row = 0; row < Rows; row++)
		{
			for (var col = 0; col < Cols; col++)
			{
				var x = offsetX + (col * (BlockWidth + BlockSpacing));
				var y = BlockOffsetY + (row * (BlockHeight + BlockSpacing));
				blocks.Add(new Block(new Rect(x, y, BlockWidth, BlockHeight)));
			}
		}

		return blocks;
	}
}
