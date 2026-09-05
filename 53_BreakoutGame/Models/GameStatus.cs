namespace BreakoutGame.Models;

/// <summary>
/// ゲームの進行状態。
/// </summary>
public enum GameStatus
{
	/// <summary>プレイ中。</summary>
	Playing,

	/// <summary>全ブロックを破壊してクリア。</summary>
	Cleared,

	/// <summary>ライフが尽きてゲームオーバー。</summary>
	GameOver,
}
