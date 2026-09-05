using System.Windows;
using BehaviorGallery.Behaviors;

namespace BehaviorGallery.Tests;

/// <summary>
/// <see cref="DragMoveCalculator"/> のテスト。
/// </summary>
public class DragMoveCalculatorTests
{
    /// <summary>
    /// パス条件: 現在位置に移動量(Vector)を加算した新しい座標を返すこと
    /// </summary>
    [Fact]
    public void CalculateNewPosition_移動量を加算した新しい座標を返す()
    {
        var current = new Point(10, 20);
        var delta = new Vector(5, -3);

        var result = DragMoveCalculator.CalculateNewPosition(current, delta);

        Assert.Equal(new Point(15, 17), result);
    }
}
