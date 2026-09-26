using System.Net.Http;
using System.Windows;
using RssReader.Services;
using RssReader.ViewModels;

namespace RssReader;

/// <summary>
/// RSSリーダーのメイン画面。DataContextにMainViewModelを設定するのみで、
/// 表示・操作ロジックはすべてViewModel側に委譲する。
/// </summary>
public partial class MainWindow : Window
{
	/// <summary>ウィンドウを初期化する。</summary>
	public MainWindow()
	{
		InitializeComponent();
		DataContext = new MainViewModel(new RssFeedClient(new HttpClient()), new ProcessBrowserLauncher());
	}
}
