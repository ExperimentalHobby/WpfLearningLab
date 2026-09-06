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
		var visited = new HashSet<(int X, int Y)>();
		// (訪問候補セル, そのセルを見つけた親セル)のペアを積む。訪問済みかどうかは
		// popして実際に処理する時点で判定する(DFSの一般的な実装作法。pushした時点で
		// visited登録すると、本来スタックが持つべき「深さ優先の巻き戻り」が発生する前に
		// 兄弟ノードを訪問済み扱いにしてしまい、探索順序の意図が曖昧になる)。
		var stack = new Stack<((int X, int Y) Cell, (int X, int Y) Parent)>();
		stack.Push((start, start));

		while (stack.Count > 0)
		{
			var (current, parent) = stack.Pop();
			if (!visited.Add(current))
			{
				continue;
			}

			if (current != start)
			{
				cameFrom[current] = parent;
			}

			visitedOrder.Add(current);

			if (current == goal)
			{
				return new MazeSolverResult(visitedOrder, MazeSolverPathBuilder.BuildPath(cameFrom, start, goal));
			}

			foreach (var neighbor in maze.GetConnectedNeighbors(current))
			{
				if (!visited.Contains(neighbor))
				{
					stack.Push((neighbor, current));
				}
			}
		}

		return new MazeSolverResult(visitedOrder, null);
	}
}
