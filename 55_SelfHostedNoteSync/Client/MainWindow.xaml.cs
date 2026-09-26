using System.Net.Http;
using System.Windows;
using SelfHostedNoteSync.Client.Services;
using SelfHostedNoteSync.Client.ViewModels;

namespace SelfHostedNoteSync.Client;

/// <summary>
/// メモ同期クライアントのメイン画面。DataContextにMainViewModelを設定するのみで、
/// 表示・操作ロジックはすべてViewModel側に委譲する。
/// </summary>
public partial class MainWindow : Window
{
	/// <summary>ウィンドウを初期化する。</summary>
	public MainWindow()
	{
		InitializeComponent();
		var viewModel = new MainViewModel(new NoteApiClient(new HttpClient()));
		DataContext = viewModel;
		Loaded += (_, _) => viewModel.LoadCommand.Execute(null);
	}
}
