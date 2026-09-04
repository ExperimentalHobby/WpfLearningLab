namespace GameOfLife.Services;

/// <summary>
/// コンウェイのライフゲームのルールエンジン。UIフレームワークに依存しない純粋なロジック。
/// 盤面は折り返さず、範囲外は近傍としてカウントしない。
/// </summary>
public class GameOfLifeEngine
{
	private bool[,] _cells;

	/// <summary>
	/// 指定したサイズの(全セル死亡状態の)盤面を初期化する。
	/// </summary>
	/// <param name="width">盤面の幅(セル数)。</param>
	/// <param name="height">盤面の高さ(セル数)。</param>
	public GameOfLifeEngine(int width, int height)
	{
		Width = width;
		Height = height;
		_cells = new bool[width, height];
	}

	/// <summary>盤面の幅(セル数)。</summary>
	public int Width { get; }

	/// <summary>盤面の高さ(セル数)。</summary>
	public int Height { get; }

	/// <summary>
	/// 指定セルが生存しているかどうかを取得する。
	/// </summary>
	public bool IsAlive(int x, int y) => _cells[x, y];

	/// <summary>
	/// 指定セルの生死を設定する。
	/// </summary>
	public void SetAlive(int x, int y, bool alive) => _cells[x, y] = alive;

	/// <summary>
	/// 指定セルの生死を反転する。
	/// </summary>
	public void ToggleCell(int x, int y) => _cells[x, y] = !_cells[x, y];

	/// <summary>
	/// 全セルを死亡状態にする。
	/// </summary>
	public void Clear() => _cells = new bool[Width, Height];

	/// <summary>
	/// ライフゲームのルール(誕生・生存・過疎・過密)に基づき盤面を1世代進める。
	/// </summary>
	public void AdvanceGeneration()
	{
		var next = new bool[Width, Height];
		for (var x = 0; x < Width; x++)
		{
			for (var y = 0; y < Height; y++)
			{
				var aliveNeighbors = CountAliveNeighbors(x, y);
				next[x, y] = _cells[x, y]
					? aliveNeighbors is 2 or 3
					: aliveNeighbors == 3;
			}
		}

		_cells = next;
	}

	private int CountAliveNeighbors(int x, int y)
	{
		var count = 0;
		for (var dx = -1; dx <= 1; dx++)
		{
			for (var dy = -1; dy <= 1; dy++)
			{
				if (dx == 0 && dy == 0)
				{
					continue;
				}

				var nx = x + dx;
				var ny = y + dy;
				if (nx >= 0 && nx < Width && ny >= 0 && ny < Height && _cells[nx, ny])
				{
					count++;
				}
			}
		}

		return count;
	}
}
