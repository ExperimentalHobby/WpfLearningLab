namespace MazeSolverVisualizer.Models;

/// <summary>
/// グリッドベースの迷路データ構造。セル間が通路として繋がっているか(壁が無いか)を隣接リストで保持する。
/// </summary>
public class Maze
{
	private readonly Dictionary<(int X, int Y), HashSet<(int X, int Y)>> _connections = [];

	/// <summary>
	/// 指定サイズの、全セルが未接続(壁だらけ)の迷路を初期化する。
	/// </summary>
	/// <param name="width">迷路の幅(セル数)。</param>
	/// <param name="height">迷路の高さ(セル数)。</param>
	public Maze(int width, int height)
	{
		Width = width;
		Height = height;
		for (var x = 0; x < width; x++)
		{
			for (var y = 0; y < height; y++)
			{
				_connections[(x, y)] = [];
			}
		}
	}

	/// <summary>迷路の幅(セル数)。</summary>
	public int Width { get; }

	/// <summary>迷路の高さ(セル数)。</summary>
	public int Height { get; }

	/// <summary>
	/// 2つのセルの間の通路を開通させる(双方向)。
	/// </summary>
	public void Connect((int X, int Y) a, (int X, int Y) b)
	{
		_connections[a].Add(b);
		_connections[b].Add(a);
	}

	/// <summary>
	/// 2つのセルが通路で直接繋がっているかどうかを返す。
	/// </summary>
	public bool IsConnected((int X, int Y) a, (int X, int Y) b) => _connections[a].Contains(b);

	/// <summary>
	/// 指定セルから通路で直接到達できる隣接セルを取得する。
	/// </summary>
	public IReadOnlyCollection<(int X, int Y)> GetConnectedNeighbors((int X, int Y) cell) => _connections[cell];

	/// <summary>
	/// 盤面内の上下左右の隣接セル座標(壁の有無に関わらず)を取得する。迷路生成アルゴリズムが未訪問セルを探すのに使う。
	/// </summary>
	public IEnumerable<(int X, int Y)> GetAdjacentCells((int X, int Y) cell)
	{
		(int dx, int dy)[] directions = [(0, -1), (0, 1), (-1, 0), (1, 0)];
		foreach (var (dx, dy) in directions)
		{
			var next = (cell.X + dx, cell.Y + dy);
			if (next.Item1 >= 0 && next.Item1 < Width && next.Item2 >= 0 && next.Item2 < Height)
			{
				yield return next;
			}
		}
	}
}
