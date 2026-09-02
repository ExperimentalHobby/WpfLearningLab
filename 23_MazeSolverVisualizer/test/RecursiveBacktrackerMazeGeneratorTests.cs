using MazeSolverVisualizer.Services;

namespace MazeSolverVisualizer.Tests;

/// <summary>
/// <see cref="RecursiveBacktrackerMazeGenerator"/> の単体テスト。
/// </summary>
public class RecursiveBacktrackerMazeGeneratorTests
{
	/// <summary>
	/// パス条件: 生成した迷路は全セルが(0,0)から通路のみで到達可能であること(スパニングツリー性)
	/// </summary>
	[Fact]
	public void Generate_全セルが起点から到達可能になる()
	{
		var generator = new RecursiveBacktrackerMazeGenerator();
		var maze = generator.Generate(5, 5, new Random(1));

		var visited = new HashSet<(int, int)> { (0, 0) };
		var queue = new Queue<(int, int)>();
		queue.Enqueue((0, 0));
		while (queue.Count > 0)
		{
			var current = queue.Dequeue();
			foreach (var neighbor in maze.GetConnectedNeighbors(current))
			{
				if (visited.Add(neighbor))
				{
					queue.Enqueue(neighbor);
				}
			}
		}

		Assert.Equal(25, visited.Count);
	}

	/// <summary>
	/// パス条件: 同一シードのRandomからは常に同じ迷路(接続関係)が生成されること
	/// </summary>
	[Fact]
	public void Generate_同一シードなら同じ迷路が生成される()
	{
		var generator = new RecursiveBacktrackerMazeGenerator();

		var maze1 = generator.Generate(5, 5, new Random(42));
		var maze2 = generator.Generate(5, 5, new Random(42));

		for (var x = 0; x < 5; x++)
		{
			for (var y = 0; y < 5; y++)
			{
				var neighbors1 = maze1.GetConnectedNeighbors((x, y)).OrderBy(c => c).ToList();
				var neighbors2 = maze2.GetConnectedNeighbors((x, y)).OrderBy(c => c).ToList();
				Assert.Equal(neighbors1, neighbors2);
			}
		}
	}

	/// <summary>
	/// パス条件: 異なるシードなら迷路の接続パターンが異なること
	/// </summary>
	[Fact]
	public void Generate_異なるシードなら迷路パターンが異なる()
	{
		var generator = new RecursiveBacktrackerMazeGenerator();

		var maze1 = generator.Generate(5, 5, new Random(1));
		var maze2 = generator.Generate(5, 5, new Random(2));

		var isSame = true;
		for (var x = 0; x < 5 && isSame; x++)
		{
			for (var y = 0; y < 5 && isSame; y++)
			{
				var neighbors1 = maze1.GetConnectedNeighbors((x, y)).OrderBy(c => c).ToList();
				var neighbors2 = maze2.GetConnectedNeighbors((x, y)).OrderBy(c => c).ToList();
				if (!neighbors1.SequenceEqual(neighbors2))
				{
					isSame = false;
				}
			}
		}

		Assert.False(isSame);
	}
}
