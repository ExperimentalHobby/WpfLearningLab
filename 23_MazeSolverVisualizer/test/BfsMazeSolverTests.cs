using MazeSolverVisualizer.Models;
using MazeSolverVisualizer.Services;

namespace MazeSolverVisualizer.Tests;

/// <summary>
/// <see cref="BfsMazeSolver"/> の単体テスト。
/// </summary>
public class BfsMazeSolverTests
{
	/// <summary>
	/// 分岐する2経路(短い経路: 2辺、長い経路: 4辺)を持つ、(0,0)から(2,0)へ向かう検証用迷路を作る。
	/// </summary>
	private static Maze BuildBranchingMaze()
	{
		var maze = new Maze(3, 2);
		// 短い経路: (0,0)-(1,0)-(2,0)
		maze.Connect((0, 0), (1, 0));
		maze.Connect((1, 0), (2, 0));
		// 長い経路: (0,0)-(0,1)-(1,1)-(2,1)-(2,0)
		maze.Connect((0, 0), (0, 1));
		maze.Connect((0, 1), (1, 1));
		maze.Connect((1, 1), (2, 1));
		maze.Connect((2, 1), (2, 0));
		return maze;
	}

	/// <summary>
	/// パス条件: 一直線の迷路でスタートからゴールまでの経路が正しく求まること
	/// </summary>
	[Fact]
	public void Solve_一直線の迷路で経路が正しく求まる()
	{
		var maze = new Maze(3, 1);
		maze.Connect((0, 0), (1, 0));
		maze.Connect((1, 0), (2, 0));
		var solver = new BfsMazeSolver();

		var result = solver.Solve(maze, (0, 0), (2, 0));

		Assert.Equal([(0, 0), (1, 0), (2, 0)], result.Path);
	}

	/// <summary>
	/// パス条件: 分岐のある迷路で最短経路が求まること
	/// </summary>
	[Fact]
	public void Solve_分岐迷路で最短経路が求まる()
	{
		var maze = BuildBranchingMaze();
		var solver = new BfsMazeSolver();

		var result = solver.Solve(maze, (0, 0), (2, 0));

		Assert.Equal([(0, 0), (1, 0), (2, 0)], result.Path);
	}

	/// <summary>
	/// パス条件: スタートとゴールが同じ場合、訪問セル1件・経路は自身のみになること
	/// </summary>
	[Fact]
	public void Solve_スタートとゴールが同じ場合経路は自身のみになる()
	{
		var maze = new Maze(3, 1);
		var solver = new BfsMazeSolver();

		var result = solver.Solve(maze, (0, 0), (0, 0));

		Assert.Equal([(0, 0)], result.Path);
		Assert.Single(result.VisitedOrder);
	}
}
