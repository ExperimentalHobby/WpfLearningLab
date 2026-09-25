using System.Net.Http;
using System.Windows;
using TaskApiWorkbench.Client.Services;
using TaskApiWorkbench.Client.ViewModels;

namespace TaskApiWorkbench.Client;

/// <summary>
/// タスク管理クライアントのメイン画面。DataContextにMainViewModelを設定するのみで、
/// 表示・操作ロジックはすべてViewModel側に委譲する。
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
		var viewModel = new MainViewModel(new TaskApiClient(new HttpClient()));
		DataContext = viewModel;
		Loaded += (_, _) => viewModel.LoadCommand.Execute(null);
	}
}
