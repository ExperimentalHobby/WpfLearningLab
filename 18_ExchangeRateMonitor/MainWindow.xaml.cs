using System.Net.Http;
using System.Windows;
using System.Windows.Threading;
using ExchangeRateMonitor.Services;
using ExchangeRateMonitor.ViewModels;

namespace ExchangeRateMonitor;

/// <summary>
/// 為替モニターアプリのメイン画面。DataContextにMainViewModelを設定し、
/// <see cref="DispatcherTimer"/>による定期更新のみをコードビハインドで担う。
/// </summary>
public partial class MainWindow : Window
{
	private static readonly TimeSpan RefreshInterval = TimeSpan.FromSeconds(30);

	private readonly MainViewModel _viewModel;
	private readonly DispatcherTimer _timer = new() { Interval = RefreshInterval };

	public MainWindow()
	{
		InitializeComponent();

		_viewModel = new MainViewModel(new FrankfurterExchangeRateApiClient(new HttpClient()));
		DataContext = _viewModel;

		_timer.Tick += (_, _) => _viewModel.RefreshAllCommand.Execute(null);
		_timer.Start();
		Closed += (_, _) => _timer.Stop();
	}
}
