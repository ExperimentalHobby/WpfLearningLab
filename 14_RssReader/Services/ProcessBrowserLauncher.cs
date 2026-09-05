using System.Diagnostics;

namespace RssReader.Services;

/// <summary>
/// <see cref="Process.Start(ProcessStartInfo)"/>(シェル実行)でURLを既定のブラウザで開く実装。
/// </summary>
public class ProcessBrowserLauncher : IBrowserLauncher
{
	/// <inheritdoc/>
	public bool Open(string url)
	{
		// RSS(外部入力)の<link>にfile:///等の危険なスキームが入っていると、
		// UseShellExecute=trueでの起動は任意のローカルファイル/プログラムの実行に
		// つながり得るため、絶対URIかつhttp/httpsスキームの場合のみ起動を許可する。
		if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
			(uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
		{
			return false;
		}

		Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
		return true;
	}
}
