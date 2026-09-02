namespace MazeSolverVisualizer.Services;

/// <summary>
/// 探索アルゴリズム共通の経路復元ロジック。各<see cref="IMazeSolver"/>実装が
/// 「どのセルからどのセルへ辿り着いたか」を記録した<c>cameFrom</c>辞書から、
/// スタートからゴールまでの経路を逆順に辿って復元する。
/// </summary>
internal static class MazeSolverPathBuilder
{
	/// <summary>
	/// <paramref name="cameFrom"/>を使ってスタートからゴールまでの経路を復元する。
	/// </summary>
	/// <param name="cameFrom">各セルの直前セルを記録した辞書。</param>
	/// <param name="start">スタート地点。</param>
	/// <param name="goal">ゴール地点。</param>
	/// <returns>スタートからゴールまでの経路。</returns>
	public static IReadOnlyList<(int X, int Y)> BuildPath(
		IReadOnlyDictionary<(int X, int Y), (int X, int Y)> cameFrom,
		(int X, int Y) start,
		(int X, int Y) goal)
	{
		var path = new List<(int X, int Y)> { goal };
		var current = goal;
		while (current != start)
		{
			current = cameFrom[current];
			path.Add(current);
		}

		path.Reverse();
		return path;
	}
}
