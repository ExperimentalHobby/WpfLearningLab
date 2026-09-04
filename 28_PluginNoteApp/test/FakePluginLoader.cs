using PluginNoteApp.Services;

namespace PluginNoteApp.Tests;

/// <summary>
/// <see cref="ViewModels.MainViewModel"/> のテスト用に、実際のDLL読込を行わない<see cref="IPluginLoader"/>実装。
/// </summary>
public class FakePluginLoader : IPluginLoader
{
	/// <summary><see cref="LoadPlugins"/>が返す結果一覧。</summary>
	public IReadOnlyList<PluginLoadResult> ResultsToReturn { get; set; } = [];

	/// <inheritdoc/>
	public IReadOnlyList<PluginLoadResult> LoadPlugins(string pluginDirectory) => ResultsToReturn;
}
