using MazeSolverVisualizer.Models;

namespace MazeSolverVisualizer.Services;

/// <summary>
/// 幅優先探索(BFS)による迷路の経路探索。全辺のコストが等しい迷路では最短経路が求まる。
/// </summary>
public class BfsMazeSolver : IMazeSolver
{
	/// <inheritdoc/>
	public string Name => "BFS(幅優先探索)";

	/// <inheritdoc/>
	public MazeSolverResult Solve(Maze maze, (int X, int Y) start, (int X, int Y) goal)
	{
		var visitedOrder = new List<(int X, int Y)>();
		var cameFrom = new Dictionary<(int X, int Y), (int X, int Y)>();
		var visited = new HashSet<(int X, int Y)> { start };
		var queue = new Queue<(int X, int Y)>();
		queue.Enqueue(start);

		while (queue.Count > 0)
		{
			var current = queue.Dequeue();
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
					queue.Enqueue(neighbor);
				}
			}
		}

		return new MazeSolverResult(visitedOrder, null);
	}
}
