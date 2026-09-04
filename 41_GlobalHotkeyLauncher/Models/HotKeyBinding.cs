namespace GlobalHotkeyLauncher.Models;

/// <summary>
/// 登録済みのホットキー1件(組み合わせ+実行内容)を表す。
/// </summary>
/// <param name="Id">Win32の<c>RegisterHotKey</c>に渡す一意なID。</param>
/// <param name="Combination">ホットキーの組み合わせ。</param>
/// <param name="Label">一覧に表示する説明。</param>
/// <param name="Target">実行対象(実行ファイルのパスまたはURL)。</param>
public sealed record HotKeyBinding(int Id, HotKeyCombination Combination, string Label, string Target);
