using GlobalHotkeyLauncher.Services;

namespace GlobalHotkeyLauncher.Tests;

/// <summary>
/// <see cref="ICommandLauncher"/> のテスト用フェイク。実プロセスは起動せず、起動対象を記録する。
/// </summary>
public class FakeCommandLauncher : ICommandLauncher
{
	/// <summary>
	/// <see cref="Launch"/>に渡された対象の一覧(呼び出し順)。
	/// </summary>
	public List<string> LaunchedTargets { get; } = [];

	/// <inheritdoc/>
	public void Launch(string target) => LaunchedTargets.Add(target);
}
