using PluginNoteApp.Contracts;

namespace PluginNoteApp.Tests;

/// <summary>
/// <see cref="ViewModels.MainViewModel"/> のテスト用のダミープラグイン。
/// </summary>
public class FakeMemoPlugin(string name, Func<string, string> process) : IMemoPlugin
{
	/// <inheritdoc/>
	public string Name { get; } = name;

	/// <inheritdoc/>
	public string Process(string memoText) => process(memoText);

	/// <summary>常に例外を投げるプラグインを作る(テスト用)。</summary>
	public static FakeMemoPlugin CreateThrowing(string name, Exception exception) =>
		new(name, _ => throw exception);
}
