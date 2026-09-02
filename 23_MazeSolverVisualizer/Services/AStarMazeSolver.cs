using MazeSolverVisualizer.Models;

namespace MazeSolverVisualizer.Services;

/// <summary>
/// A*探索による迷路の経路探索。マンハッタン距離をヒューリスティックとして使い、
/// ダイクストラ法よりゴール方向を優先的に探索することで無駄な探索を減らす。
/// </summary>
public class AStarMazeSolver : IMazeSolver
{
	/// <inheritdoc/>
	public string Name => "A*探索";

	/// <inheritdoc/>
	public MazeSolverResult Solve(Maze maze, (int X, int Y) start, (int X, int Y) goal)
	{
		var visitedOrder = new List<(int X, int Y)>();
		var cameFrom = new Dictionary<(int X, int Y), (int X, int Y)>();
		var costSoFar = new Dictionary<(int X, int Y), int> { [start] = 0 };
		var visited = new HashSet<(int X, int Y)>();
		var queue = new PriorityQueue<(int X, int Y), int>();
		queue.Enqueue(start, Heuristic(start, goal));

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
					queue.Enqueue(neighbor, newCost + Heuristic(neighbor, goal));
				}
			}
		}

		return new MazeSolverResult(visitedOrder, null);
	}

	private static int Heuristic((int X, int Y) a, (int X, int Y) b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
}
