namespace GlobalHotkeyLauncher.Services;

/// <summary>
/// ホットキー発火時に実行するコマンド(アプリ起動・URLを開く等)を抽象化する。
/// </summary>
public interface ICommandLauncher
{
	/// <summary>
	/// 指定した対象(実行ファイルのパスまたはURL)を起動する。
	/// </summary>
	/// <param name="target">実行対象。</param>
	/// <returns>起動できた場合は<see langword="true"/>、対象が空/空白の場合や起動に失敗した場合は<see langword="false"/>。</returns>
	bool Launch(string target);
}
