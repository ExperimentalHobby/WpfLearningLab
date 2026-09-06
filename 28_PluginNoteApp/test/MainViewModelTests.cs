using PluginNoteApp.Services;
using PluginNoteApp.ViewModels;

namespace PluginNoteApp.Tests;

/// <summary>
/// <see cref="MainViewModel"/> の単体テスト。
/// </summary>
public class MainViewModelTests
{
	/// <summary>
	/// パス条件: 読み込みに成功したプラグインが、Pluginsに反映されること
	/// </summary>
	[Fact]
	public void コンストラクタ_読込に成功したプラグインがPluginsに反映される()
	{
		var plugin = new FakeMemoPlugin("テストプラグイン", text => text);
		var loader = new FakePluginLoader
		{
			ResultsToReturn = [new PluginLoadResult("テストプラグイン", plugin, null)],
		};

		var viewModel = new MainViewModel(loader, "dummy");

		Assert.Single(viewModel.Plugins);
		Assert.Same(plugin, viewModel.Plugins[0]);
	}

	/// <summary>
	/// パス条件: 読み込みに失敗したプラグインが、LoadErrorsに反映されること
	/// </summary>
	[Fact]
	public void コンストラクタ_読込に失敗したプラグインがLoadErrorsに反映される()
	{
		var loader = new FakePluginLoader
		{
			ResultsToReturn = [new PluginLoadResult("Broken.dll", null, "破損したDLLです。")],
		};

		var viewModel = new MainViewModel(loader, "dummy");

		Assert.Empty(viewModel.Plugins);
		var error = Assert.Single(viewModel.LoadErrors);
		Assert.Contains("Broken.dll", error);
		Assert.Contains("破損したDLLです。", error);
	}

	/// <summary>
	/// パス条件: プラグイン未選択の場合、RunPluginCommandが実行不可になること
	/// </summary>
	[Fact]
	public void RunPluginCommand_プラグイン未選択の場合CanExecuteがfalseになる()
	{
		var viewModel = new MainViewModel(new FakePluginLoader(), "dummy");

		Assert.False(viewModel.RunPluginCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: RunPluginCommand実行で、選択中プラグインのProcess結果がPluginOutputに反映されること
	/// </summary>
	[Fact]
	public void RunPluginCommand_実行すると選択中プラグインの結果がPluginOutputに反映される()
	{
		var plugin = new FakeMemoPlugin("大文字化", text => text.ToUpperInvariant());
		var loader = new FakePluginLoader
		{
			ResultsToReturn = [new PluginLoadResult("大文字化", plugin, null)],
		};
		var viewModel = new MainViewModel(loader, "dummy");
		viewModel.MemoText = "hello";
		viewModel.SelectedPlugin = viewModel.Plugins[0];

		viewModel.RunPluginCommand.Execute(null);

		Assert.Equal("HELLO", viewModel.PluginOutput);
	}

	/// <summary>
	/// パス条件: 選択中プラグインのProcessが例外を投げても、ホストアプリはクラッシュせず
	/// PluginOutputにエラーメッセージが表示されること(プラグイン機構として致命的な欠陥の修正確認)
	/// </summary>
	[Fact]
	public void RunPluginCommand_プラグインが例外を投げてもクラッシュせずエラーが表示される()
	{
		var plugin = FakeMemoPlugin.CreateThrowing("壊れたプラグイン", new InvalidOperationException("プラグイン内部エラー"));
		var loader = new FakePluginLoader
		{
			ResultsToReturn = [new PluginLoadResult("壊れたプラグイン", plugin, null)],
		};
		var viewModel = new MainViewModel(loader, "dummy");
		viewModel.MemoText = "hello";
		viewModel.SelectedPlugin = viewModel.Plugins[0];

		viewModel.RunPluginCommand.Execute(null);

		Assert.Contains("プラグイン内部エラー", viewModel.PluginOutput);
	}
}
