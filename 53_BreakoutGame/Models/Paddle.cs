using System.Windows;

namespace BreakoutGame.Models;

/// <summary>
/// パドルの状態(左上座標・サイズ)。
/// </summary>
/// <param name="Position">左上座標。</param>
/// <param name="Width">幅。</param>
/// <param name="Height">高さ。</param>
public record Paddle(Point Position, double Width, double Height)
{
	/// <summary>
	/// パドルの矩形範囲。
	/// </summary>
	public Rect Bounds => new(Position, new Size(Width, Height));
}
