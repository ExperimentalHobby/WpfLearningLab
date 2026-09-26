using System.Windows;
using BreakoutGame.Engine;
using BreakoutGame.Models;

namespace BreakoutGame.Tests;

/// <summary>
/// <see cref="CollisionDetector"/>の単体テスト。
/// </summary>
public class CollisionDetectorTests
{
	/// <summary>
	/// パス条件: ボールが左壁(矩形)に当たると水平方向速度が反転すること。
	/// </summary>
	[Fact]
	public void TryReflect_HitLeftSide_ReflectsHorizontalVelocity()
	{
		// 壁を表す矩形がボールの左側に接している
		var wall = new Rect(-100, -100, 100, 300);
		var ball = new Ball(new Point(2, 0), new Vector(-100, 0), 5);

		var reflected = CollisionDetector.TryReflect(ball, wall, out var velocity);

		Assert.True(reflected);
		Assert.Equal(100, velocity.X);
		Assert.Equal(0, velocity.Y);
	}

	/// <summary>
	/// パス条件: ボールが上壁(矩形)に当たると垂直方向速度が反転すること。
	/// </summary>
	[Fact]
	public void TryReflect_HitTopSide_ReflectsVerticalVelocity()
	{
		var wall = new Rect(-100, -100, 300, 100);
		var ball = new Ball(new Point(0, 2), new Vector(0, -100), 5);

		var reflected = CollisionDetector.TryReflect(ball, wall, out var velocity);

		Assert.True(reflected);
		Assert.Equal(0, velocity.X);
		Assert.Equal(100, velocity.Y);
	}

	/// <summary>
	/// パス条件: ボールがブロックに当たると反射し、trueが返ること(衝突検知)。
	/// </summary>
	[Fact]
	public void TryReflect_HitBlock_ReturnsTrueAndReflects()
	{
		var block = new Rect(0, 0, 50, 20);
		var ball = new Ball(new Point(25, -2), new Vector(0, 100), 5);

		var reflected = CollisionDetector.TryReflect(ball, block, out var velocity);

		Assert.True(reflected);
		Assert.Equal(-100, velocity.Y);
	}

	/// <summary>
	/// パス条件: ボールが矩形から十分離れている場合は衝突しないこと。
	/// </summary>
	[Fact]
	public void TryReflect_NotOverlapping_ReturnsFalse()
	{
		var block = new Rect(0, 0, 50, 20);
		var ball = new Ball(new Point(25, -100), new Vector(0, 100), 5);

		var reflected = CollisionDetector.TryReflect(ball, block, out _);

		Assert.False(reflected);
	}

	/// <summary>
	/// パス条件: ボール中心が縦長矩形の内部に完全に入っている場合、矩形の薄い方の軸(この場合X軸)で
	/// 反射されること(最近接点ベースの距離だけでは矩形の縦横比を無視してしまう不具合の確認)。
	/// </summary>
	[Fact]
	public void TryReflect_BallFullyInsideNarrowRect_ReflectsAlongShorterAxis()
	{
		// 幅20・高さ50の縦長矩形。ボール中心は矩形の中心と一致し、完全に内包されている
		var rect = new Rect(0, 0, 20, 50);
		var ball = new Ball(new Point(10, 25), new Vector(50, 50), 3);

		var reflected = CollisionDetector.TryReflect(ball, rect, out var velocity);

		Assert.True(reflected);
		// 幅(20)の方が高さ(50)より薄いため、X軸方向(vx)が反射されるべき
		Assert.Equal(-50, velocity.X);
		Assert.Equal(50, velocity.Y);
	}

	/// <summary>
	/// パス条件: パドル中央に当たるとほぼ真上方向(水平速度が0に近い)に反射すること。
	/// </summary>
	[Fact]
	public void ReflectOffPaddle_HitCenter_ReflectsNearlyStraightUp()
	{
		var paddle = new Paddle(new Point(0, 100), 80, 12);
		var ball = new Ball(new Point(40, 100), new Vector(0, 100), 5);

		var velocity = CollisionDetector.ReflectOffPaddle(ball, paddle, 200);

		Assert.True(velocity.Y < 0);
		Assert.True(Math.Abs(velocity.X) < 1e-6);
	}

	/// <summary>
	/// パス条件: パドルの端に当たると、中央に当たった場合より水平速度成分が大きくなること(反射角度が変わる)。
	/// </summary>
	[Fact]
	public void ReflectOffPaddle_HitEdge_HasLargerHorizontalComponentThanCenter()
	{
		var paddle = new Paddle(new Point(0, 100), 80, 12);
		var centerBall = new Ball(new Point(40, 100), new Vector(0, 100), 5);
		var edgeBall = new Ball(new Point(78, 100), new Vector(0, 100), 5);

		var centerVelocity = CollisionDetector.ReflectOffPaddle(centerBall, paddle, 200);
		var edgeVelocity = CollisionDetector.ReflectOffPaddle(edgeBall, paddle, 200);

		Assert.True(Math.Abs(edgeVelocity.X) > Math.Abs(centerVelocity.X));
		Assert.True(edgeVelocity.Y < 0);
	}
}
