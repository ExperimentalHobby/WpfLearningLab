using System.Windows;
using LogStreamAggregator.Services;
using LogStreamAggregator.ViewModels;

namespace LogStreamAggregator;

/// <summary>
/// ログストリーム集計ツールのメイン画面。
/// </summary>
public partial class MainWindow : Window
{
	/// <summary>ウィンドウを初期化する。</summary>
	public MainWindow()
	{
		InitializeComponent();
		DataContext = new MainViewModel(new WpfUiDispatcher());
	}
}
