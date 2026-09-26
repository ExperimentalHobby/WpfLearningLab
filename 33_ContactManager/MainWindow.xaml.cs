using System.Windows;
using ContactManager.ViewModels;

namespace ContactManager;

/// <summary>
/// 連絡先管理アプリのメイン画面。
/// DIコンテナから<see cref="MainViewModel"/>を注入される(<see cref="App"/>参照)。
/// </summary>
public partial class MainWindow : Window
{
	/// <summary>ウィンドウを初期化する。</summary>
	/// <param name="viewModel">DataContextに設定するViewModel。</param>
	public MainWindow(MainViewModel viewModel)
	{
		InitializeComponent();
		DataContext = viewModel;
	}
}
