using GlobalHotkeyLauncher.Services;

namespace GlobalHotkeyLauncher.Tests;

/// <summary>
/// <see cref="ProcessCommandLauncher"/>のテスト。
/// </summary>
public class ProcessCommandLauncherTests
{
	/// <summary>
	/// パス条件: 対象が空文字の場合、Process.Startを呼ばずfalseを返すこと。
	/// </summary>
	[Fact]
	public void Launch_対象が空文字の場合はfalseを返す()
	{
		var launcher = new ProcessCommandLauncher();

		Assert.False(launcher.Launch(string.Empty));
	}

	/// <summary>
	/// パス条件: 対象が空白のみの場合、falseを返すこと。
	/// </summary>
	[Fact]
	public void Launch_対象が空白のみの場合はfalseを返す()
	{
		var launcher = new ProcessCommandLauncher();

		Assert.False(launcher.Launch("   "));
	}

	/// <summary>
	/// パス条件: 存在しないパスを指定した場合、例外にならずfalseを返すこと
	/// (WM_HOTKEY処理中のクラッシュを防ぐための回帰テスト)。
	/// </summary>
	[Fact]
	public void Launch_存在しないパスの場合は例外にならずfalseを返す()
	{
		var launcher = new ProcessCommandLauncher();
		var nonExistentPath = $@"C:\{Guid.NewGuid():N}\{Guid.NewGuid():N}.exe";

		var exception = Record.Exception(() => launcher.Launch(nonExistentPath));

		Assert.Null(exception);
		Assert.False(launcher.Launch(nonExistentPath));
	}
}
