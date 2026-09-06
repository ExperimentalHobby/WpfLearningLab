using MazeSolverVisualizer.Services;

namespace MazeSolverVisualizer.Tests;

/// <summary>
/// <see cref="MazeSolverPathBuilder"/> の単体テスト。
/// </summary>
public class MazeSolverPathBuilderTests
{
	/// <summary>
	/// パス条件: cameFromが正しく繋がっている場合、スタートからゴールまでの経路を復元できること
	/// </summary>
	[Fact]
	public void BuildPath_正常なcameFromから経路を復元できる()
	{
		var cameFrom = new Dictionary<(int X, int Y), (int X, int Y)>
		{
			[(1, 0)] = (0, 0),
			[(2, 0)] = (1, 0),
		};

		var path = MazeSolverPathBuilder.BuildPath(cameFrom, (0, 0), (2, 0));

		Assert.Equal([(0, 0), (1, 0), (2, 0)], path);
	}

	/// <summary>
	/// パス条件: cameFromにゴールへ辿る途中のセル情報が欠落している場合、
	/// KeyNotFoundExceptionではなくInvalidOperationExceptionを投げること
	/// </summary>
	[Fact]
	public void BuildPath_cameFromの情報が欠落しているとInvalidOperationExceptionを投げる()
	{
		var cameFrom = new Dictionary<(int X, int Y), (int X, int Y)>
		{
			[(2, 0)] = (1, 0),
			// (1, 0)の直前セル情報が欠落している
		};

		Assert.Throws<InvalidOperationException>(() => MazeSolverPathBuilder.BuildPath(cameFrom, (0, 0), (2, 0)));
	}

	/// <summary>
	/// パス条件: cameFromが循環している場合、無限ループにならずInvalidOperationExceptionを投げること
	/// </summary>
	[Fact]
	public void BuildPath_cameFromが循環しているとInvalidOperationExceptionを投げる()
	{
		var cameFrom = new Dictionary<(int X, int Y), (int X, int Y)>
		{
			[(2, 0)] = (1, 0),
			[(1, 0)] = (2, 0), // (1,0)と(2,0)が互いを指す循環
		};

		Assert.Throws<InvalidOperationException>(() => MazeSolverPathBuilder.BuildPath(cameFrom, (0, 0), (2, 0)));
	}
}
