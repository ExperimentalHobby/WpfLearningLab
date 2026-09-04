using MazeSolverVisualizer.Models;

namespace MazeSolverVisualizer.Services;

/// <summary>
/// 迷路の自動生成を担う抽象。
/// </summary>
public interface IMazeGenerator
{
	/// <summary>
	/// 指定サイズの迷路を生成する。
	/// </summary>
	/// <param name="width">迷路の幅(セル数)。</param>
	/// <param name="height">迷路の高さ(セル数)。</param>
	/// <param name="random">乱数生成器。同じシードを与えれば常に同じ迷路が生成される。</param>
	Maze Generate(int width, int height, Random random);
}
