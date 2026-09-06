using System.IO;
using System.Windows;
using PluginNoteApp.Services;
using PluginNoteApp.ViewModels;

namespace PluginNoteApp;

/// <summary>
/// プラグイン対応メモアプリのメイン画面。DataContextにMainViewModelを設定するのみで、
/// 表示・操作ロジックはすべてViewModel側に委譲する。
/// プラグインDLLは実行ファイルと同じフォルダの<c>Plugins</c>サブフォルダから読み込む。
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
		var pluginDirectory = Path.Combine(AppContext.BaseDirectory, "Plugins");
		var pluginLoader = new MefPluginLoader();
		DataContext = new MainViewModel(pluginLoader, pluginDirectory);

		Closed += (_, _) => pluginLoader.Dispose();
	}
}
