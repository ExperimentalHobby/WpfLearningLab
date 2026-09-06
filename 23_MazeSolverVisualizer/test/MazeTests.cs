using MazeSolverVisualizer.Models;

namespace MazeSolverVisualizer.Tests;

/// <summary>
/// <see cref="Maze"/> の単体テスト。
/// </summary>
public class MazeTests
{
	/// <summary>
	/// パス条件: Connectに範囲外のセルを渡すとArgumentOutOfRangeExceptionを投げること
	/// </summary>
	[Fact]
	public void Connect_範囲外のセルを渡すとArgumentOutOfRangeExceptionを投げる()
	{
		var maze = new Maze(3, 3);

		Assert.Throws<ArgumentOutOfRangeException>(() => maze.Connect((-1, 0), (0, 0)));
		Assert.Throws<ArgumentOutOfRangeException>(() => maze.Connect((0, 0), (3, 0)));
	}

	/// <summary>
	/// パス条件: IsConnectedに範囲外のセルを渡すとArgumentOutOfRangeExceptionを投げること
	/// </summary>
	[Fact]
	public void IsConnected_範囲外のセルを渡すとArgumentOutOfRangeExceptionを投げる()
	{
		var maze = new Maze(3, 3);

		Assert.Throws<ArgumentOutOfRangeException>(() => maze.IsConnected((0, -1), (0, 0)));
	}

	/// <summary>
	/// パス条件: GetConnectedNeighborsに範囲外のセルを渡すとArgumentOutOfRangeExceptionを投げること
	/// </summary>
	[Fact]
	public void GetConnectedNeighbors_範囲外のセルを渡すとArgumentOutOfRangeExceptionを投げる()
	{
		var maze = new Maze(3, 3);

		Assert.Throws<ArgumentOutOfRangeException>(() => maze.GetConnectedNeighbors((3, 3)));
	}
}
