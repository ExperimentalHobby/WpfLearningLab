using System.Windows;
using LocalChatApp.Services;
using LocalChatApp.ViewModels;

namespace LocalChatApp;

/// <summary>
/// 簡易チャットアプリのメイン画面。DataContextにMainViewModelを設定するのみで、
/// 表示・操作ロジックはすべてViewModel側に委譲する。
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
		DataContext = new MainViewModel(new TcpChatServer(), new TcpChatClient(), new WpfUiDispatcher());
	}
}
