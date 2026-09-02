using System.Windows;
using LocalTaskScheduler.Services;
using LocalTaskScheduler.ViewModels;

namespace LocalTaskScheduler;

/// <summary>
/// ローカルタスクスケジューラアプリのメイン画面。
/// バックグラウンドの<see cref="ThreadingTimerTicker"/>が一定間隔で<see cref="MainViewModel.CheckDueTasks"/>を呼び出す。
/// </summary>
public partial class MainWindow : Window
{
	private readonly IBackgroundTicker _ticker = new ThreadingTimerTicker();
	private readonly IUiDispatcher _dispatcher = new WpfUiDispatcher();
	private readonly MainViewModel _viewModel;

	public MainWindow()
	{
		InitializeComponent();
		_viewModel = new MainViewModel(new ToastNotifier());
		DataContext = _viewModel;

		_ticker.Ticked += now => _dispatcher.Invoke(() => _viewModel.CheckDueTasks(now));
		_ticker.Start(TimeSpan.FromSeconds(1));
		Closed += (_, _) => _ticker.Dispose();
	}
}
