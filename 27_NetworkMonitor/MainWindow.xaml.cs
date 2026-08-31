using System.Windows;
using System.Windows.Threading;
using NetworkMonitor.Services;
using NetworkMonitor.ViewModels;

namespace NetworkMonitor;

/// <summary>
/// ネットワーク帯域モニターアプリのメイン画面。
/// 一定間隔ごとに<see cref="MainViewModel.Sample"/>を呼び出すDispatcherTimerを持つ。
/// </summary>
public partial class MainWindow : Window
{
	private readonly PerformanceCounterNetworkBandwidthSampler _sampler = new();
	private readonly MainViewModel _viewModel;
	private readonly DispatcherTimer _sampleTimer = new() { Interval = TimeSpan.FromSeconds(1) };

	public MainWindow()
	{
		InitializeComponent();
		_viewModel = new MainViewModel(_sampler);
		DataContext = _viewModel;
		_sampleTimer.Tick += (_, _) => _viewModel.Sample();
		_sampleTimer.Start();
		Closed += (_, _) =>
		{
			_sampleTimer.Stop();
			_sampler.Dispose();
		};
	}
}
