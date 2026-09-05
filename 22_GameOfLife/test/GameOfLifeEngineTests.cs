using GameOfLife.Services;

namespace GameOfLife.Tests;

/// <summary>
/// <see cref="GameOfLifeEngine"/> の単体テスト。
/// </summary>
public class GameOfLifeEngineTests
{
	/// <summary>
	/// パス条件: 隣接する生存セルが1つ以下の生存セルは、過疎により次世代で死ぬこと
	/// </summary>
	[Fact]
	public void AdvanceGeneration_隣接1つ以下の生存セルは過疎で死ぬ()
	{
		var engine = new GameOfLifeEngine(3, 3);
		engine.SetAlive(1, 1, true);
		engine.SetAlive(0, 0, true);

		engine.AdvanceGeneration();

		Assert.False(engine.IsAlive(1, 1));
	}

	/// <summary>
	/// パス条件: 隣接する生存セルが2〜3の生存セルは、次世代も生存し続けること
	/// </summary>
	[Theory]
	[InlineData(2)]
	[InlineData(3)]
	public void AdvanceGeneration_隣接2から3の生存セルは生存し続ける(int aliveNeighborCount)
	{
		var engine = new GameOfLifeEngine(5, 5);
		engine.SetAlive(2, 2, true);
		var neighborCoords = new (int X, int Y)[] { (1, 1), (2, 1), (3, 1) };
		for (var i = 0; i < aliveNeighborCount; i++)
		{
			engine.SetAlive(neighborCoords[i].X, neighborCoords[i].Y, true);
		}

		engine.AdvanceGeneration();

		Assert.True(engine.IsAlive(2, 2));
	}

	/// <summary>
	/// パス条件: 隣接する生存セルが4つ以上の生存セルは、過密により次世代で死ぬこと
	/// </summary>
	[Fact]
	public void AdvanceGeneration_隣接4つ以上の生存セルは過密で死ぬ()
	{
		var engine = new GameOfLifeEngine(5, 5);
		engine.SetAlive(2, 2, true);
		engine.SetAlive(1, 1, true);
		engine.SetAlive(2, 1, true);
		engine.SetAlive(3, 1, true);
		engine.SetAlive(1, 2, true);

		engine.AdvanceGeneration();

		Assert.False(engine.IsAlive(2, 2));
	}

	/// <summary>
	/// パス条件: 隣接する生存セルがちょうど3つの死亡セルは、次世代で誕生すること
	/// </summary>
	[Fact]
	public void AdvanceGeneration_隣接3つの死亡セルは誕生する()
	{
		var engine = new GameOfLifeEngine(3, 3);
		engine.SetAlive(0, 0, true);
		engine.SetAlive(1, 0, true);
		engine.SetAlive(0, 1, true);

		engine.AdvanceGeneration();

		Assert.True(engine.IsAlive(1, 1));
	}

	/// <summary>
	/// パス条件: ブリンカー(横3連続)が1世代で縦3連続に反転すること
	/// </summary>
	[Fact]
	public void AdvanceGeneration_ブリンカーが1世代で縦横反転する()
	{
		var engine = new GameOfLifeEngine(5, 5);
		engine.SetAlive(1, 2, true);
		engine.SetAlive(2, 2, true);
		engine.SetAlive(3, 2, true);

		engine.AdvanceGeneration();

		Assert.False(engine.IsAlive(1, 2));
		Assert.False(engine.IsAlive(3, 2));
		Assert.True(engine.IsAlive(2, 1));
		Assert.True(engine.IsAlive(2, 2));
		Assert.True(engine.IsAlive(2, 3));
	}

	/// <summary>
	/// パス条件: ToggleCellで対象セルの生死が反転すること
	/// </summary>
	[Fact]
	public void ToggleCell_対象セルの生死が反転する()
	{
		var engine = new GameOfLifeEngine(3, 3);

		engine.ToggleCell(1, 1);
		Assert.True(engine.IsAlive(1, 1));

		engine.ToggleCell(1, 1);
		Assert.False(engine.IsAlive(1, 1));
	}

	/// <summary>
	/// パス条件: Clearで全セルが死亡状態になること
	/// </summary>
	[Fact]
	public void Clear_全セルが死亡状態になる()
	{
		var engine = new GameOfLifeEngine(3, 3);
		engine.SetAlive(0, 0, true);
		engine.SetAlive(1, 1, true);

		engine.Clear();

		Assert.False(engine.IsAlive(0, 0));
		Assert.False(engine.IsAlive(1, 1));
	}

	/// <summary>
	/// パス条件: 盤面の角のセルは範囲外を近傍として数えず、正しく誕生判定されること
	/// </summary>
	[Fact]
	public void AdvanceGeneration_盤面端は範囲外を近傍として数えない()
	{
		var engine = new GameOfLifeEngine(2, 2);
		// (0,0)を除く3セルを生存させる。(0,0)の実際の近傍は(1,0)(0,1)(1,1)の3つのみで
		// 全て生存しているため誕生条件(隣接ちょうど3)を満たす。
		engine.SetAlive(1, 0, true);
		engine.SetAlive(0, 1, true);
		engine.SetAlive(1, 1, true);

		engine.AdvanceGeneration();

		Assert.True(engine.IsAlive(0, 0));
	}

	/// <summary>
	/// パス条件: IsAliveに範囲外の座標を指定するとArgumentOutOfRangeExceptionを投げること
	/// </summary>
	[Theory]
	[InlineData(-1, 0)]
	[InlineData(0, -1)]
	[InlineData(3, 0)]
	[InlineData(0, 3)]
	public void IsAlive_範囲外の座標を指定するとArgumentOutOfRangeExceptionを投げる(int x, int y)
	{
		var engine = new GameOfLifeEngine(3, 3);

		Assert.Throws<ArgumentOutOfRangeException>(() => engine.IsAlive(x, y));
	}

	/// <summary>
	/// パス条件: SetAliveに範囲外の座標を指定するとArgumentOutOfRangeExceptionを投げること
	/// </summary>
	[Theory]
	[InlineData(-1, 0)]
	[InlineData(0, -1)]
	[InlineData(3, 0)]
	[InlineData(0, 3)]
	public void SetAlive_範囲外の座標を指定するとArgumentOutOfRangeExceptionを投げる(int x, int y)
	{
		var engine = new GameOfLifeEngine(3, 3);

		Assert.Throws<ArgumentOutOfRangeException>(() => engine.SetAlive(x, y, true));
	}

	/// <summary>
	/// パス条件: ToggleCellに範囲外の座標を指定するとArgumentOutOfRangeExceptionを投げること
	/// </summary>
	[Theory]
	[InlineData(-1, 0)]
	[InlineData(0, -1)]
	[InlineData(3, 0)]
	[InlineData(0, 3)]
	public void ToggleCell_範囲外の座標を指定するとArgumentOutOfRangeExceptionを投げる(int x, int y)
	{
		var engine = new GameOfLifeEngine(3, 3);

		Assert.Throws<ArgumentOutOfRangeException>(() => engine.ToggleCell(x, y));
	}
}
