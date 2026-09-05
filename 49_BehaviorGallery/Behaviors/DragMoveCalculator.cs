using System.Windows;

namespace BehaviorGallery.Behaviors;

/// <summary>
/// ドラッグ移動の座標計算ロジック。UIイベント配線から切り出した純粋な計算部分。
/// </summary>
public static class DragMoveCalculator
{
    /// <summary>
    /// 現在位置にドラッグ移動量を加算した新しい座標を計算する。
    /// </summary>
    public static Point CalculateNewPosition(Point current, Vector delta)
    {
        return current + delta;
    }
}
