using MazeSolverVisualizer.Models;

namespace MazeSolverVisualizer.Services;

/// <summary>
/// 深さ優先探索(DFS)による迷路の経路探索。見つかる経路は必ずしも最短経路にはならない。
/// </summary>
public class DfsMazeSolver : IMazeSolver
{
	/// <inheritdoc/>
	public string Name => "DFS(深さ優先探索)";

	/// <inheritdoc/>
	public MazeSolverResult Solve(Maze maze, (int X, int Y) start, (int X, int Y) goal)
	{
		var visitedOrder = new List<(int X, int Y)>();
		var cameFrom = new Dictionary<(int X, int Y), (int X, int Y)>();
		var visited = new HashSet<(int X, int Y)> { start };
		var stack = new Stack<(int X, int Y)>();
		stack.Push(start);

		while (stack.Count > 0)
		{
			var current = stack.Pop();
			visitedOrder.Add(current);

			if (current == goal)
			{
				return new MazeSolverResult(visitedOrder, MazeSolverPathBuilder.BuildPath(cameFrom, start, goal));
			}

			foreach (var neighbor in maze.GetConnectedNeighbors(current))
			{
				if (visited.Add(neighbor))
				{
					cameFrom[neighbor] = current;
					stack.Push(neighbor);
				}
			}
		}

		return new MazeSolverResult(visitedOrder, null);
	}
}
