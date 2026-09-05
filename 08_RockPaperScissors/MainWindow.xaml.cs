using System.Windows;
using System.Windows.Controls;

namespace RockPaperScissors;

/// <summary>
/// じゃんけんゲームのメインウィンドウ。
/// 勝敗判定・記録の管理は <see cref="RockPaperScissorsEngine"/> に委譲し、
/// ボタン操作の受け取りと表示更新のみを担当する薄いコードビハインド。
/// </summary>
public partial class MainWindow : Window
{
	private readonly RockPaperScissorsEngine _engine = new();

	/// <summary>
	/// ウィンドウを初期化する。
	/// </summary>
	public MainWindow()
	{
		InitializeComponent();
	}

	/// <summary>
	/// グー/チョキ/パーいずれかのボタン押下時の処理。
	/// </summary>
	private void HandButton_Click(object sender, RoutedEventArgs e)
	{
		if (sender is not Button { Tag: string tag } || !Enum.TryParse<Hand>(tag, out var playerHand))
		{
			return;
		}

		var (cpuHand, result) = _engine.Play(playerHand);
		UpdateDisplay(playerHand, cpuHand, result);
	}

	/// <summary>
	/// 「リセット」ボタン押下時の処理。勝敗記録をクリアする。
	/// </summary>
	private void ResetButton_Click(object sender, RoutedEventArgs e)
	{
		_engine.ResetCounts();
		PlayerHandTextBlock.Text = "あなた: -";
		CpuHandTextBlock.Text = "CPU: -";
		ResultTextBlock.Text = "手を選んでください";
		UpdateScoreDisplay();
	}

	/// <summary>
	/// 対戦結果を画面に反映する。
	/// </summary>
	private void UpdateDisplay(Hand playerHand, Hand cpuHand, GameResult result)
	{
		PlayerHandTextBlock.Text = $"あなた: {ToJapanese(playerHand)}";
		CpuHandTextBlock.Text = $"CPU: {ToJapanese(cpuHand)}";
		ResultTextBlock.Text = result switch
		{
			GameResult.Win => "あなたの勝ち!",
			GameResult.Lose => "あなたの負け...",
			_ => "あいこ",
		};
		UpdateScoreDisplay();
	}

	/// <summary>
	/// 勝敗数・連勝記録の表示を更新する。
	/// </summary>
	private void UpdateScoreDisplay()
	{
		ScoreTextBlock.Text = $"勝ち:{_engine.WinCount}　負け:{_engine.LoseCount}　あいこ:{_engine.DrawCount}";
		StreakTextBlock.Text = $"連勝: {_engine.CurrentWinStreak}";
	}

	/// <summary>
	/// 手の名称を日本語表記に変換する。
	/// </summary>
	private static string ToJapanese(Hand hand) => hand switch
	{
		Hand.Rock => "グー",
		Hand.Scissors => "チョキ",
		Hand.Paper => "パー",
		// Hand列挙型はRock/Scissors/Paperの3値のみを想定しており、ここには到達しないはず。
		// (byte)99等の不正なキャストで到達した場合は、ToString()で曖昧に処理するのではなく
		// 早期に気づけるよう例外を送出する。
		_ => throw new ArgumentOutOfRangeException(nameof(hand), hand, "未対応の手です。"),
	};
}
