using RssReader.Services;

namespace RssReader.Tests;

/// <summary>
/// <see cref="RssReader.ViewModels.MainViewModel"/> のテスト用に、実際にブラウザを起動しない<see cref="IBrowserLauncher"/>実装。
/// </summary>
public class FakeBrowserLauncher : IBrowserLauncher
{
	/// <summary>直近に<see cref="Open"/>で渡されたURL(テスト用)。</summary>
	public string? LastOpenedUrl { get; private set; }

	/// <summary>Openの戻り値。安全でないURLの拒否をシミュレートするテストで false に設定する。</summary>
	public bool ReturnValue { get; set; } = true;

	/// <inheritdoc/>
	public bool Open(string url)
	{
		LastOpenedUrl = url;
		return ReturnValue;
	}
}
