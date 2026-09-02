using MazeSolverVisualizer.Models;

namespace MazeSolverVisualizer.Services;

/// <summary>
/// 迷路の経路探索アルゴリズムの抽象。
/// </summary>
public interface IMazeSolver
{
	/// <summary>アルゴリズム名(UI表示・選択に使う)。</summary>
	string Name { get; }

	/// <summary>
	/// スタートからゴールまでの経路を探索する。
	/// </summary>
	/// <param name="maze">探索対象の迷路。</param>
	/// <param name="start">スタート地点。</param>
	/// <param name="goal">ゴール地点。</param>
	MazeSolverResult Solve(Maze maze, (int X, int Y) start, (int X, int Y) goal);
}
