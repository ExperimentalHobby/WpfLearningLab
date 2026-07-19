namespace RockPaperScissors.Tests;

/// <summary>
/// <see cref="RockPaperScissorsEngine"/> の勝敗判定・記録管理に関するテスト。
/// </summary>
public class RockPaperScissorsEngineTests
{
	/// <summary>
	/// パス条件: 同じ手同士(グー対グー)のJudgeがDrawを返すこと。
	/// </summary>
	[Fact]
	public void Judge_SameHands_ReturnsDraw()
	{
		var result = RockPaperScissorsEngine.Judge(Hand.Rock, Hand.Rock);

		Assert.Equal(GameResult.Draw, result);
	}

	/// <summary>
	/// パス条件: グーはチョキに勝つのでJudgeがWinを返すこと。
	/// </summary>
	[Fact]
	public void Judge_RockBeatsScissors_ReturnsWin()
	{
		var result = RockPaperScissorsEngine.Judge(Hand.Rock, Hand.Scissors);

		Assert.Equal(GameResult.Win, result);
	}

	/// <summary>
	/// パス条件: チョキはパーに勝つのでJudgeがWinを返すこと。
	/// </summary>
	[Fact]
	public void Judge_ScissorsBeatsPaper_ReturnsWin()
	{
		var result = RockPaperScissorsEngine.Judge(Hand.Scissors, Hand.Paper);

		Assert.Equal(GameResult.Win, result);
	}

	/// <summary>
	/// パス条件: パーはグーに勝つのでJudgeがWinを返すこと。
	/// </summary>
	[Fact]
	public void Judge_PaperBeatsRock_ReturnsWin()
	{
		var result = RockPaperScissorsEngine.Judge(Hand.Paper, Hand.Rock);

		Assert.Equal(GameResult.Win, result);
	}

	/// <summary>
	/// パス条件: グーはパーに負けるのでJudgeがLoseを返すこと。
	/// </summary>
	[Fact]
	public void Judge_RockLosesToPaper_ReturnsLose()
	{
		var result = RockPaperScissorsEngine.Judge(Hand.Rock, Hand.Paper);

		Assert.Equal(GameResult.Lose, result);
	}

	/// <summary>
	/// パス条件: チョキはグーに負けるのでJudgeがLoseを返すこと。
	/// </summary>
	[Fact]
	public void Judge_ScissorsLosesToRock_ReturnsLose()
	{
		var result = RockPaperScissorsEngine.Judge(Hand.Scissors, Hand.Rock);

		Assert.Equal(GameResult.Lose, result);
	}

	/// <summary>
	/// パス条件: パーはチョキに負けるのでJudgeがLoseを返すこと。
	/// </summary>
	[Fact]
	public void Judge_PaperLosesToScissors_ReturnsLose()
	{
		var result = RockPaperScissorsEngine.Judge(Hand.Paper, Hand.Scissors);

		Assert.Equal(GameResult.Lose, result);
	}

	/// <summary>
	/// パス条件: 既定のコンストラクタでPlayを繰り返し呼んでも、CPUの手が常に定義済みの値になること。
	/// </summary>
	[Fact]
	public void Play_DefaultConstructor_CpuHandIsAlwaysValid()
	{
		var engine = new RockPaperScissorsEngine();

		for (var i = 0; i < 100; i++)
		{
			var (cpuHand, _) = engine.Play(Hand.Rock);
			Assert.True(Enum.IsDefined(cpuHand));
		}
	}

	/// <summary>
	/// パス条件: 勝った場合、WinCountが増えCurrentWinStreakも増えること。
	/// </summary>
	[Fact]
	public void Play_Win_IncrementsWinCountAndStreak()
	{
		var engine = new RockPaperScissorsEngine(() => Hand.Scissors);

		engine.Play(Hand.Rock);

		Assert.Equal(1, engine.WinCount);
		Assert.Equal(0, engine.LoseCount);
		Assert.Equal(0, engine.DrawCount);
		Assert.Equal(1, engine.CurrentWinStreak);
	}

	/// <summary>
	/// パス条件: 負けた場合、LoseCountが増えCurrentWinStreakが0にリセットされること。
	/// </summary>
	[Fact]
	public void Play_Lose_IncrementsLoseCountAndResetsStreak()
	{
		var winEngine = new RockPaperScissorsEngine(() => Hand.Scissors);
		winEngine.Play(Hand.Rock);

		var loseEngine = new RockPaperScissorsEngine(() => Hand.Paper);
		loseEngine.Play(Hand.Rock);

		Assert.Equal(0, loseEngine.WinCount);
		Assert.Equal(1, loseEngine.LoseCount);
		Assert.Equal(0, loseEngine.CurrentWinStreak);
	}

	/// <summary>
	/// パス条件: あいこの場合、DrawCountが増えCurrentWinStreakは変化しないこと。
	/// </summary>
	[Fact]
	public void Play_Draw_IncrementsDrawCountAndKeepsStreak()
	{
		var engine = new RockPaperScissorsEngine(() => Hand.Scissors);
		engine.Play(Hand.Rock);

		var drawEngine = new RockPaperScissorsEngine(() => Hand.Rock);
		drawEngine.Play(Hand.Rock);

		Assert.Equal(0, drawEngine.WinCount);
		Assert.Equal(1, drawEngine.DrawCount);
		Assert.Equal(0, drawEngine.CurrentWinStreak);
	}

	/// <summary>
	/// パス条件: 連続で勝つと連勝数が積み上がること。
	/// </summary>
	[Fact]
	public void Play_ConsecutiveWins_StreakIncrementsEachTime()
	{
		var engine = new RockPaperScissorsEngine(() => Hand.Scissors);

		engine.Play(Hand.Rock);
		engine.Play(Hand.Rock);

		Assert.Equal(2, engine.WinCount);
		Assert.Equal(2, engine.CurrentWinStreak);
	}

	/// <summary>
	/// パス条件: ResetCountsを呼ぶと全カウントが0に戻ること。
	/// </summary>
	[Fact]
	public void ResetCounts_AllCountsBackToZero()
	{
		var engine = new RockPaperScissorsEngine(() => Hand.Scissors);
		engine.Play(Hand.Rock);

		engine.ResetCounts();

		Assert.Equal(0, engine.WinCount);
		Assert.Equal(0, engine.LoseCount);
		Assert.Equal(0, engine.DrawCount);
		Assert.Equal(0, engine.CurrentWinStreak);
	}
}
