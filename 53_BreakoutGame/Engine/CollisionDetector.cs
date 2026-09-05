using System.Windows;
using BreakoutGame.Models;

namespace BreakoutGame.Engine;

/// <summary>
/// 円(ボール)と矩形(壁・パドル・ブロック)の当たり判定・反射計算を担う。UI非依存の純粋ロジック。
/// </summary>
public static class CollisionDetector
{
	/// <summary>
	/// ボールが矩形に重なっているかを判定し、重なっていれば貫入の浅い軸の速度成分を反転させた
	/// 反射後速度を求める。
	/// </summary>
	/// <param name="ball">対象のボール。</param>
	/// <param name="rect">衝突判定対象の矩形。</param>
	/// <param name="reflectedVelocity">衝突した場合の反射後速度。衝突しなかった場合は元の速度。</param>
	/// <returns>衝突した場合は<see langword="true"/>。</returns>
	public static bool TryReflect(Ball ball, Rect rect, out Vector reflectedVelocity)
	{
		var closestX = Math.Clamp(ball.Position.X, rect.Left, rect.Right);
		var closestY = Math.Clamp(ball.Position.Y, rect.Top, rect.Bottom);

		var dx = ball.Position.X - closestX;
		var dy = ball.Position.Y - closestY;
		var distanceSquared = (dx * dx) + (dy * dy);

		if (distanceSquared > ball.Radius * ball.Radius)
		{
			reflectedVelocity = ball.Velocity;
			return false;
		}

		var overlapX = ball.Radius - Math.Abs(dx);
		var overlapY = ball.Radius - Math.Abs(dy);

		var vx = ball.Velocity.X;
		var vy = ball.Velocity.Y;

		if (overlapX < overlapY)
		{
			vx = -vx;
		}
		else
		{
			vy = -vy;
		}

		reflectedVelocity = new Vector(vx, vy);
		return true;
	}

	/// <summary>
	/// パドルに当たった際の反射速度を求める。パドル中央からの当たり位置に応じて反射角度が変わる
	/// (中央ほど真上寄り、端に近いほど斜め方向)。
	/// </summary>
	/// <param name="ball">対象のボール。</param>
	/// <param name="paddle">衝突したパドル。</param>
	/// <param name="speed">反射後の速さ(ピクセル/秒)。</param>
	public static Vector ReflectOffPaddle(Ball ball, Paddle paddle, double speed)
	{
		const double maxAngle = Math.PI / 3; // 端に当たったときの最大反射角(60度)

		var paddleCenterX = paddle.Position.X + (paddle.Width / 2);
		var hitPosition = (ball.Position.X - paddleCenterX) / (paddle.Width / 2);
		hitPosition = Math.Clamp(hitPosition, -1.0, 1.0);

		var angle = hitPosition * maxAngle;
		var vx = speed * Math.Sin(angle);
		var vy = -speed * Math.Cos(angle);

		return new Vector(vx, vy);
	}
}
