using System.Windows;
using LogStreamAggregator.Services;
using LogStreamAggregator.ViewModels;

namespace LogStreamAggregator;

/// <summary>
/// ログストリーム集計ツールのメイン画面。
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
		DataContext = new MainViewModel(new WpfUiDispatcher());
	}
}
