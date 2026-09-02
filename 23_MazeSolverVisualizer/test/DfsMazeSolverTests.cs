using MazeSolverVisualizer.Models;
using MazeSolverVisualizer.Services;

namespace MazeSolverVisualizer.Tests;

/// <summary>
/// <see cref="DfsMazeSolver"/> の単体テスト。
/// </summary>
public class DfsMazeSolverTests
{
	/// <summary>
	/// パス条件: 一直線の迷路でスタートからゴールまでの経路が正しく求まること
	/// </summary>
	[Fact]
	public void Solve_一直線の迷路で経路が正しく求まる()
	{
		var maze = new Maze(3, 1);
		maze.Connect((0, 0), (1, 0));
		maze.Connect((1, 0), (2, 0));
		var solver = new DfsMazeSolver();

		var result = solver.Solve(maze, (0, 0), (2, 0));

		Assert.Equal([(0, 0), (1, 0), (2, 0)], result.Path);
	}

	/// <summary>
	/// パス条件: 分岐のある迷路で、スタートからゴールまで連続して繋がる経路が求まること
	/// (DFSは必ずしも最短経路にはならない)
	/// </summary>
	[Fact]
	public void Solve_分岐迷路でゴールへ到達する経路が求まる()
	{
		var maze = new Maze(3, 2);
		maze.Connect((0, 0), (1, 0));
		maze.Connect((1, 0), (2, 0));
		maze.Connect((0, 0), (0, 1));
		maze.Connect((0, 1), (1, 1));
		maze.Connect((1, 1), (2, 1));
		maze.Connect((2, 1), (2, 0));
		var solver = new DfsMazeSolver();

		var result = solver.Solve(maze, (0, 0), (2, 0));

		Assert.NotNull(result.Path);
		Assert.Equal((0, 0), result.Path![0]);
		Assert.Equal((2, 0), result.Path[^1]);
		for (var i = 0; i < result.Path.Count - 1; i++)
		{
			Assert.True(maze.IsConnected(result.Path[i], result.Path[i + 1]));
		}
	}

	/// <summary>
	/// パス条件: スタートとゴールが同じ場合、訪問セル1件・経路は自身のみになること
	/// </summary>
	[Fact]
	public void Solve_スタートとゴールが同じ場合経路は自身のみになる()
	{
		var maze = new Maze(3, 1);
		var solver = new DfsMazeSolver();

		var result = solver.Solve(maze, (0, 0), (0, 0));

		Assert.Equal([(0, 0)], result.Path);
		Assert.Single(result.VisitedOrder);
	}
}
