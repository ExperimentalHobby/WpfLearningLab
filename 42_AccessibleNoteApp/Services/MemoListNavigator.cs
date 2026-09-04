using System.Windows.Input;

namespace AccessibleNoteApp.Services;

/// <summary>
/// メモ一覧のキーボードによる選択移動(↑/↓/Home/End)を計算する純粋ロジック。
/// UIに依存せず、単体テストで境界条件を確認できる。
/// </summary>
public static class MemoListNavigator
{
	/// <summary>
	/// 現在の選択Index・件数・押されたキーから、次に選択すべきIndexを求める。
	/// </summary>
	/// <param name="currentIndex">現在の選択Index(未選択の場合は-1)。</param>
	/// <param name="itemCount">一覧の件数。</param>
	/// <param name="key">押されたキー。</param>
	/// <returns>
	/// 次に選択すべきIndex。ナビゲーション対象外のキー、または件数が0の場合は<see langword="null"/>。
	/// </returns>
	public static int? GetNextIndex(int currentIndex, int itemCount, Key key)
	{
		if (itemCount == 0)
		{
			return null;
		}

		return key switch
		{
			Key.Down => Math.Min(currentIndex + 1, itemCount - 1),
			Key.Up => Math.Max(currentIndex - 1, 0),
			Key.Home => 0,
			Key.End => itemCount - 1,
			_ => null,
		};
	}
}
