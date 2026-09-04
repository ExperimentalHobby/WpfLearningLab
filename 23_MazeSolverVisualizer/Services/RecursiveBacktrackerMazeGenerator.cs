using MazeSolverVisualizer.Models;

namespace MazeSolverVisualizer.Services;

/// <summary>
/// 穴掘り法(再帰的バックトラッカー)による迷路生成。
/// (0,0)を起点に、ランダムな未訪問隣接セルへ通路を掘り進め、行き止まりではバックトラックする。
/// 生成される迷路は全セルが単一の経路で繋がるスパニングツリーになる。
/// </summary>
public class RecursiveBacktrackerMazeGenerator : IMazeGenerator
{
	/// <inheritdoc/>
	public Maze Generate(int width, int height, Random random)
	{
		var maze = new Maze(width, height);
		var visited = new HashSet<(int X, int Y)> { (0, 0) };
		var stack = new Stack<(int X, int Y)>();
		stack.Push((0, 0));

		while (stack.Count > 0)
		{
			var current = stack.Peek();
			var unvisitedNeighbors = maze.GetAdjacentCells(current).Where(cell => !visited.Contains(cell)).ToList();

			if (unvisitedNeighbors.Count == 0)
			{
				stack.Pop();
				continue;
			}

			var next = unvisitedNeighbors[random.Next(unvisitedNeighbors.Count)];
			maze.Connect(current, next);
			visited.Add(next);
			stack.Push(next);
		}

		return maze;
	}
}
