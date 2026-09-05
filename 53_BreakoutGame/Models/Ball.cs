using System.Windows;

namespace BreakoutGame.Models;

/// <summary>
/// ボールの状態(位置・速度・半径)。
/// </summary>
/// <param name="Position">中心座標。</param>
/// <param name="Velocity">速度(ピクセル/秒)。</param>
/// <param name="Radius">半径。</param>
public record Ball(Point Position, Vector Velocity, double Radius);
