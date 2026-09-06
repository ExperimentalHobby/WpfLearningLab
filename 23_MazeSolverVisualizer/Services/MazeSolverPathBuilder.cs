namespace MazeSolverVisualizer.Services;

/// <summary>
/// 探索アルゴリズム共通の経路復元ロジック。各<see cref="IMazeSolver"/>実装が
/// 「どのセルからどのセルへ辿り着いたか」を記録した<c>cameFrom</c>辞書から、
/// スタートからゴールまでの経路を逆順に辿って復元する。
/// </summary>
public static class MazeSolverPathBuilder
{
	/// <summary>
	/// <paramref name="cameFrom"/>を使ってスタートからゴールまでの経路を復元する。
	/// </summary>
	/// <param name="cameFrom">各セルの直前セルを記録した辞書。</param>
	/// <param name="start">スタート地点。</param>
	/// <param name="goal">ゴール地点。</param>
	/// <returns>スタートからゴールまでの経路。</returns>
	/// <exception cref="InvalidOperationException">
	/// <paramref name="cameFrom"/>にセルの直前情報が欠落している、または循環しておりスタートに
	/// 辿り着けない場合(いずれも探索アルゴリズムの実装ミスを示す)。
	/// </exception>
	public static IReadOnlyList<(int X, int Y)> BuildPath(
		IReadOnlyDictionary<(int X, int Y), (int X, int Y)> cameFrom,
		(int X, int Y) start,
		(int X, int Y) goal)
	{
		var path = new List<(int X, int Y)> { goal };
		var visitedWhileBacktracking = new HashSet<(int X, int Y)> { goal };
		var current = goal;
		while (current != start)
		{
			if (!cameFrom.TryGetValue(current, out var previous))
			{
				throw new InvalidOperationException(
					$"経路復元に失敗しました: セル{current}の直前セル情報がcameFromに存在しません。");
			}

			if (!visitedWhileBacktracking.Add(previous))
			{
				throw new InvalidOperationException(
					$"経路復元中にcameFromの循環を検出しました(セル{previous})。スタートに到達できません。");
			}

			current = previous;
			path.Add(current);
		}

		path.Reverse();
		return path;
	}
}
