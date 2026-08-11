using RssReader.Services;

namespace RssReader.Tests;

/// <summary>
/// <see cref="MainViewModel"/> のテスト用に、実際にブラウザを起動しない<see cref="IBrowserLauncher"/>実装。
/// </summary>
public class FakeBrowserLauncher : IBrowserLauncher
{
	public string? LastOpenedUrl { get; private set; }

	public void Open(string url)
	{
		LastOpenedUrl = url;
	}
}
