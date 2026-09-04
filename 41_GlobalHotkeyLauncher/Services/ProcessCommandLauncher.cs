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
	public void Launch(string target)
	{
		Process.Start(new ProcessStartInfo(target) { UseShellExecute = true });
	}
}
