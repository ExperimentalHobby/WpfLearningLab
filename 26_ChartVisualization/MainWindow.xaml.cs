using System.Windows;
using ChartVisualization.ViewModels;

namespace ChartVisualization;

/// <summary>
/// 簡易グラフ描画アプリのメイン画面。DataContextにMainViewModelを設定するのみで、
/// 表示・操作ロジックはすべてViewModel側に委譲する。
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
		DataContext = new MainViewModel();
	}
}
