using System.Windows;

namespace BreakoutGame.Models;

/// <summary>
/// ブロックの状態。破壊済みかどうかを可変で保持する。
/// </summary>
public class Block(Rect bounds)
{
	/// <summary>
	/// ブロックの矩形範囲。
	/// </summary>
	public Rect Bounds { get; } = bounds;

	/// <summary>
	/// 破壊済みかどうか。
	/// </summary>
	public bool IsDestroyed { get; set; }
}
