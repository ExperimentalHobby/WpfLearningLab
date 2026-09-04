using MazeSolverVisualizer.Models;

namespace MazeSolverVisualizer.Services;

/// <summary>
/// ダイクストラ法による迷路の経路探索。各辺のコストは1として累積コストが最小の経路を求める。
/// </summary>
public class DijkstraMazeSolver : IMazeSolver
{
	/// <inheritdoc/>
	public string Name => "ダイクストラ法";

	/// <inheritdoc/>
	public MazeSolverResult Solve(Maze maze, (int X, int Y) start, (int X, int Y) goal)
	{
		var visitedOrder = new List<(int X, int Y)>();
		var cameFrom = new Dictionary<(int X, int Y), (int X, int Y)>();
		var costSoFar = new Dictionary<(int X, int Y), int> { [start] = 0 };
		var visited = new HashSet<(int X, int Y)>();
		var queue = new PriorityQueue<(int X, int Y), int>();
		queue.Enqueue(start, 0);

		while (queue.Count > 0)
		{
			var current = queue.Dequeue();
			if (!visited.Add(current))
			{
				continue;
			}

			visitedOrder.Add(current);

			if (current == goal)
			{
				return new MazeSolverResult(visitedOrder, MazeSolverPathBuilder.BuildPath(cameFrom, start, goal));
			}

			foreach (var neighbor in maze.GetConnectedNeighbors(current))
			{
				var newCost = costSoFar[current] + 1;
				if (!costSoFar.TryGetValue(neighbor, out var existingCost) || newCost < existingCost)
				{
					costSoFar[neighbor] = newCost;
					cameFrom[neighbor] = current;
					queue.Enqueue(neighbor, newCost);
				}
			}
		}

		return new MazeSolverResult(visitedOrder, null);
	}
}
