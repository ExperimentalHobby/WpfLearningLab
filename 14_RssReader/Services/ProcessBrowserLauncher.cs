using System.Diagnostics;

namespace RssReader.Services;

/// <summary>
/// <see cref="Process.Start(ProcessStartInfo)"/>(シェル実行)でURLを既定のブラウザで開く実装。
/// </summary>
public class ProcessBrowserLauncher : IBrowserLauncher
{
	/// <inheritdoc/>
	public void Open(string url)
	{
		Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
	}
}
