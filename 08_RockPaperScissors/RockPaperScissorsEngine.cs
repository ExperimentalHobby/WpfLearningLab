namespace RockPaperScissors;

/// <summary>
/// じゃんけんの勝敗判定・CPUの手の決定・勝敗記録の管理を行うエンジン。
/// </summary>
public class RockPaperScissorsEngine
{
	private readonly Func<Hand> _cpuHandSelector;

	/// <summary>
	/// 累計の勝ち数。
	/// </summary>
	public int WinCount { get; private set; }

	/// <summary>
	/// 累計の負け数。
	/// </summary>
	public int LoseCount { get; private set; }

	/// <summary>
	/// 累計のあいこ数。
	/// </summary>
	public int DrawCount { get; private set; }

	/// <summary>
	/// 現在の連勝数。負けた時点で0にリセットされる。あいこでは変化しない。
	/// </summary>
	public int CurrentWinStreak { get; private set; }

	/// <summary>
	/// CPUの手を実際の乱数で決定するエンジンを初期化する。
	/// </summary>
	public RockPaperScissorsEngine() : this(() => (Hand)Random.Shared.Next(3))
	{
	}

	/// <summary>
	/// CPUの手の決定方法を差し替えてエンジンを初期化する(主にテスト用)。
	/// </summary>
	/// <param name="cpuHandSelector">CPUの手を返す関数。</param>
	public RockPaperScissorsEngine(Func<Hand> cpuHandSelector)
	{
		_cpuHandSelector = cpuHandSelector;
	}

	/// <summary>
	/// 1回分のじゃんけんを実行する。CPUの手を決定し、勝敗判定と記録の更新を行う。
	/// </summary>
	/// <param name="playerHand">プレイヤーの手。</param>
	/// <returns>CPUの手と勝敗結果の組。</returns>
	public (Hand CpuHand, GameResult Result) Play(Hand playerHand)
	{
		var cpuHand = _cpuHandSelector();
		var result = Judge(playerHand, cpuHand);
		Record(result);
		return (cpuHand, result);
	}

	/// <summary>
	/// 勝敗結果を記録に反映する。
	/// </summary>
	private void Record(GameResult result)
	{
		switch (result)
		{
			case GameResult.Win:
				WinCount++;
				CurrentWinStreak++;
				break;
			case GameResult.Lose:
				LoseCount++;
				CurrentWinStreak = 0;
				break;
			case GameResult.Draw:
				DrawCount++;
				break;
		}
	}

	/// <summary>
	/// 勝敗記録(WinCount/LoseCount/DrawCount/CurrentWinStreak)をすべて0に戻す。
	/// </summary>
	public void ResetCounts()
	{
		WinCount = 0;
		LoseCount = 0;
		DrawCount = 0;
		CurrentWinStreak = 0;
	}

	/// <summary>
	/// プレイヤーの手とCPUの手から勝敗を判定する。
	/// </summary>
	/// <param name="player">プレイヤーの手。</param>
	/// <param name="cpu">CPUの手。</param>
	public static GameResult Judge(Hand player, Hand cpu)
	{
		if (player == cpu)
		{
			return GameResult.Draw;
		}

		var playerWins = (player == Hand.Rock && cpu == Hand.Scissors)
			|| (player == Hand.Scissors && cpu == Hand.Paper)
			|| (player == Hand.Paper && cpu == Hand.Rock);

		return playerWins ? GameResult.Win : GameResult.Lose;
	}
}
