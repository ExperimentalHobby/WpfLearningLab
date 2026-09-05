using RssReader.Services;

namespace RssReader.Tests;

/// <summary>
/// <see cref="MainViewModel"/> のテスト用に、実際にブラウザを起動しない<see cref="IBrowserLauncher"/>実装。
/// </summary>
public class FakeBrowserLauncher : IBrowserLauncher
{
	public string? LastOpenedUrl { get; private set; }

	/// <summary>Openの戻り値。安全でないURLの拒否をシミュレートするテストで false に設定する。</summary>
	public bool ReturnValue { get; set; } = true;

	public bool Open(string url)
	{
		LastOpenedUrl = url;
		return ReturnValue;
	}
}
