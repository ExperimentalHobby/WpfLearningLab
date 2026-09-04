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
	void Launch(string target);
}
