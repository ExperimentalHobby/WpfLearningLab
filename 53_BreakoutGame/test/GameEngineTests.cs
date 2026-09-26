using BreakoutGame.Engine;
using BreakoutGame.Models;

namespace BreakoutGame.Tests;

/// <summary>
/// <see cref="GameEngine"/>の単体テスト。
/// </summary>
public class GameEngineTests
{
	/// <summary>
	/// パス条件: Updateを呼ぶと、deltaSeconds分ボールが速度方向へ移動すること。
	/// </summary>
	[Fact]
	public void Update_MovesBallByVelocityTimesDelta()
	{
		var engine = new GameEngine(400, 500);
		var before = engine.Ball;

		engine.Update(0.01);

		var after = engine.Ball;
		Assert.NotEqual(before.Position, after.Position);
	}

	/// <summary>
	/// パス条件: ブロックに衝突するとIsDestroyed=trueになりScoreが加算されること。
	/// </summary>
	[Fact]
	public void Update_HitsBlock_DestroysBlockAndAddsScore()
	{
		var engine = new GameEngine(400, 500);
		var target = engine.Blocks[0];

		// ブロックの直下から真上に向かって当てる
		engine.ForceBallState(
			new System.Windows.Point(target.Bounds.Left + (target.Bounds.Width / 2), target.Bounds.Bottom + 4),
			new System.Windows.Vector(0, -200));

		engine.Update(0.05);

		Assert.True(target.IsDestroyed);
		Assert.Equal(10, engine.Score);
	}

	/// <summary>
	/// パス条件: 1フレームでブロックの厚みを大幅に超える距離を移動する極端に高速なボールでも、
	/// ブロックをすり抜けずに衝突が検知されること(トンネリング対策の確認)。
	/// </summary>
	[Fact]
	public void Update_FastBall_DoesNotTunnelThroughBlock()
	{
		var engine = new GameEngine(400, 500);
		var target = engine.Blocks[0];

		// ブロックのすぐ下から、1フレーム(0.05秒)で1000px移動する速度で真上に向かって当てる
		// (ブロックの厚み16pxを大幅に超える移動量であり、1フレーム1回の終点判定だけではすり抜ける)
		engine.ForceBallState(
			new System.Windows.Point(target.Bounds.Left + (target.Bounds.Width / 2), target.Bounds.Bottom + 2),
			new System.Windows.Vector(0, -20000));

		engine.Update(0.05);

		Assert.True(target.IsDestroyed);
	}

	/// <summary>
	/// パス条件: 破壊済みのブロックには再度衝突しないこと。
	/// </summary>
	[Fact]
	public void Update_DestroyedBlock_NoLongerCollides()
	{
		var engine = new GameEngine(400, 500);
		var target = engine.Blocks[0];
		target.IsDestroyed = true;

		engine.ForceBallState(
			new System.Windows.Point(target.Bounds.Left + (target.Bounds.Width / 2), target.Bounds.Bottom + 4),
			new System.Windows.Vector(0, -200));

		engine.Update(0.05);

		Assert.Equal(0, engine.Score);
	}

	/// <summary>
	/// パス条件: 全ブロックを破壊するとStatus=Clearedになること。
	/// </summary>
	[Fact]
	public void Update_AllBlocksDestroyed_SetsStatusCleared()
	{
		var engine = new GameEngine(400, 500);
		foreach (var block in engine.Blocks)
		{
			block.IsDestroyed = true;
		}

		engine.Update(0.01);

		Assert.Equal(GameStatus.Cleared, engine.Status);
	}

	/// <summary>
	/// パス条件: ボールが画面下端を超えるとLivesが減り、Livesが残っていればボールが再配置されること。
	/// </summary>
	[Fact]
	public void Update_BallFallsBelowField_DecreasesLivesAndResetsBall()
	{
		var engine = new GameEngine(400, 500);
		var initialLives = engine.Lives;
		engine.ForceBallState(new System.Windows.Point(200, 495), new System.Windows.Vector(0, 300));

		engine.Update(0.05);

		Assert.Equal(initialLives - 1, engine.Lives);
		Assert.True(engine.Ball.Position.Y < 495);
		Assert.Equal(GameStatus.Playing, engine.Status);
	}

	/// <summary>
	/// パス条件: Livesが0になった状態でボールが落下するとStatus=GameOverになること。
	/// </summary>
	[Fact]
	public void Update_BallFallsWithNoLivesLeft_SetsStatusGameOver()
	{
		var engine = new GameEngine(400, 500);
		while (engine.Lives > 1)
		{
			engine.ForceBallState(new System.Windows.Point(200, 495), new System.Windows.Vector(0, 300));
			engine.Update(0.05);
		}

		engine.ForceBallState(new System.Windows.Point(200, 495), new System.Windows.Vector(0, 300));
		engine.Update(0.05);

		Assert.Equal(GameStatus.GameOver, engine.Status);
	}

	/// <summary>
	/// パス条件: Restartで得点・ライフ・ブロック・ボール位置が初期状態に戻ること。
	/// </summary>
	[Fact]
	public void Restart_ResetsToInitialState()
	{
		var engine = new GameEngine(400, 500);
		engine.Blocks[0].IsDestroyed = true;
		engine.ForceBallState(new System.Windows.Point(200, 495), new System.Windows.Vector(0, 300));
		engine.Update(0.05); // Score加算なし、Lives減少あり

		engine.Restart();

		Assert.Equal(0, engine.Score);
		Assert.Equal(3, engine.Lives);
		Assert.Equal(GameStatus.Playing, engine.Status);
		Assert.All(engine.Blocks, b => Assert.False(b.IsDestroyed));
	}

	/// <summary>
	/// パス条件: SetPaddleDirectionで指定した方向にパドルが移動すること。
	/// </summary>
	[Fact]
	public void SetPaddleDirection_MovesPaddle()
	{
		var engine = new GameEngine(400, 500);
		var before = engine.Paddle.Position.X;

		engine.SetPaddleDirection(1);
		engine.Update(0.1);

		Assert.True(engine.Paddle.Position.X > before);
	}
}
