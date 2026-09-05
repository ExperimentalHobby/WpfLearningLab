using RssReader.Services;

namespace RssReader.Tests;

/// <summary>
/// <see cref="ProcessBrowserLauncher"/> のURLスキーム検証に関するテスト。
/// 実際にブラウザ/プロセスを起動する経路(http/https)まではテストせず、
/// 危険なスキームが拒否されることのみを検証する。
/// </summary>
public class ProcessBrowserLauncherTests
{
	/// <summary>
	/// パス条件: file:スキームのURLはProcess.Startを呼ばずにfalseを返すこと
	/// (RSSの&lt;link&gt;に file:/// 等が入ると、UseShellExecute=trueで任意の
	/// ローカルファイル/プログラムが実行され得るため)
	/// </summary>
	[Fact]
	public void Open_fileスキームのURLは起動を拒否してfalseを返す()
	{
		var launcher = new ProcessBrowserLauncher();

		var result = launcher.Open("file:///C:/Windows/System32/calc.exe");

		Assert.False(result);
	}

	/// <summary>
	/// パス条件: 相対URI等、絶対URIとして解釈できない文字列はfalseを返すこと
	/// </summary>
	[Theory]
	[InlineData("")]
	[InlineData("not a url")]
	[InlineData("javascript:alert(1)")]
	public void Open_不正または危険なURLはfalseを返す(string url)
	{
		var launcher = new ProcessBrowserLauncher();

		var result = launcher.Open(url);

		Assert.False(result);
	}
}
