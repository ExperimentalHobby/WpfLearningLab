using System.ComponentModel;
using System.Diagnostics;

namespace GlobalHotkeyLauncher.Services;

/// <summary>
/// <see cref="Process.Start(ProcessStartInfo)"/>を使い、実行ファイルのパスまたはURLを起動する実装。
/// <c>UseShellExecute</c>を有効にすることで、実行ファイル・URLどちらもエクスプローラー/既定アプリ経由で
/// 同じ方法で起動できる。
/// </summary>
public sealed class ProcessCommandLauncher : ICommandLauncher
{
	/// <inheritdoc/>
	public bool Launch(string target)
	{
		if (string.IsNullOrWhiteSpace(target))
		{
			return false;
		}

		try
		{
			Process.Start(new ProcessStartInfo(target) { UseShellExecute = true });
			return true;
		}
		catch (Win32Exception)
		{
			// 存在しないパス・関連付けなし・権限不足等。呼び出し元(WM_HOTKEY処理中のWndProc)を
			// クラッシュさせないよう、ここで捕捉しfalseを返す。
			return false;
		}
	}
}
