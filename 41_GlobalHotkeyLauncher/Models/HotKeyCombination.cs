using System.Windows.Input;

namespace GlobalHotkeyLauncher.Models;

/// <summary>
/// ホットキーの組み合わせ(修飾キー+キー)を表す値。
/// 修飾キーの値(<see cref="System.Windows.Input.ModifierKeys"/>)はWin32の MOD_ALT/MOD_CONTROL/MOD_SHIFT/MOD_WIN
/// とビット値が一致するため、登録時にそのままキャストして利用できる。
/// </summary>
/// <param name="Modifiers">修飾キー(Ctrl/Alt/Shift/Win)の組み合わせ。</param>
/// <param name="Key">通常キー。</param>
public readonly record struct HotKeyCombination(ModifierKeys Modifiers, Key Key)
{
	/// <summary>
	/// <see cref="ToDisplayString"/>をXAMLバインディングから参照するための算出プロパティ。
	/// </summary>
	public string DisplayString => ToDisplayString();

	/// <summary>
	/// 修飾キーを表示順(Ctrl→Alt→Shift→Win)に並べ、"Ctrl+Alt+L"のような表示文字列を組み立てる。
	/// </summary>
	public string ToDisplayString()
	{
		var parts = new List<string>();
		if (Modifiers.HasFlag(ModifierKeys.Control))
		{
			parts.Add("Ctrl");
		}
		if (Modifiers.HasFlag(ModifierKeys.Alt))
		{
			parts.Add("Alt");
		}
		if (Modifiers.HasFlag(ModifierKeys.Shift))
		{
			parts.Add("Shift");
		}
		if (Modifiers.HasFlag(ModifierKeys.Windows))
		{
			parts.Add("Win");
		}
		parts.Add(Key.ToString());

		return string.Join("+", parts);
	}

	/// <summary>
	/// ホットキーとして登録可能かどうかを検証する。
	/// キーが未選択、または修飾キーが1つも指定されていない場合は無効とする
	/// (修飾キー無しの単一キーを奪うと通常のタイピングを妨げてしまうため)。
	/// </summary>
	/// <param name="errorMessage">無効な場合の理由。有効な場合は<see langword="null"/>。</param>
	/// <returns>登録可能な場合は<see langword="true"/>。</returns>
	public bool Validate(out string? errorMessage)
	{
		if (Key == Key.None)
		{
			errorMessage = "キーを選択してください。";
			return false;
		}
		if (Modifiers == ModifierKeys.None)
		{
			errorMessage = "修飾キー(Ctrl/Alt/Shift/Win)を1つ以上選択してください。";
			return false;
		}

		errorMessage = null;
		return true;
	}
}
