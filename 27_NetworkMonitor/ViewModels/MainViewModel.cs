using NetworkMonitor.Models;
using NetworkMonitor.Services;
using OxyPlot;

namespace NetworkMonitor.ViewModels;

/// <summary>
/// ネットワークインターフェースの選択・監視開始/停止・帯域グラフ表示を行うメイン画面のViewModel。
/// </summary>
public class MainViewModel : ObservableObject
{
	private readonly INetworkBandwidthSampler _sampler;
	private readonly BandwidthHistory _history;

	private string? _selectedInterface;
	private bool _isMonitoring;
	private PlotModel _plotModel;

	/// <summary>
	/// ViewModelを初期化する。
	/// </summary>
	/// <param name="sampler">帯域計測処理。</param>
	/// <param name="maxSampleCount">保持する最大サンプル数(既定60件)。</param>
	public MainViewModel(INetworkBandwidthSampler sampler, int maxSampleCount = 60)
	{
		_sampler = sampler;
		_history = new BandwidthHistory(maxSampleCount);
		NetworkInterfaces = sampler.GetInstanceNames();
		StartMonitoringCommand = new RelayCommand(StartMonitoring, CanStartMonitoring);
		StopMonitoringCommand = new RelayCommand(StopMonitoring, CanStopMonitoring);
		_plotModel = BandwidthChartModelBuilder.Build(_history.Samples);
	}

	/// <summary>計測可能なネットワークインターフェースのインスタンス名一覧。</summary>
	public IReadOnlyList<string> NetworkInterfaces { get; }

	/// <summary>選択中のネットワークインターフェース。変更すると履歴がクリアされる。</summary>
	public string? SelectedInterface
	{
		get => _selectedInterface;
		set
		{
			if (SetProperty(ref _selectedInterface, value))
			{
				_history.Clear();
				RebuildPlotModel();
				StartMonitoringCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>監視中かどうか。</summary>
	public bool IsMonitoring
	{
		get => _isMonitoring;
		private set
		{
			if (SetProperty(ref _isMonitoring, value))
			{
				StartMonitoringCommand.RaiseCanExecuteChanged();
				StopMonitoringCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>現在の履歴から組み立てられた<see cref="PlotModel"/>。</summary>
	public PlotModel PlotModel
	{
		get => _plotModel;
		private set => SetProperty(ref _plotModel, value);
	}

	/// <summary>監視を開始するコマンド。</summary>
	public RelayCommand StartMonitoringCommand { get; }

	/// <summary>監視を停止するコマンド。</summary>
	public RelayCommand StopMonitoringCommand { get; }

	/// <summary>
	/// 現在選択中のインターフェースを計測し、履歴・<see cref="PlotModel"/>を更新する。
	/// 監視中でない、またはインターフェース未選択の場合は何もしない。
	/// View側の<see cref="System.Windows.Threading.DispatcherTimer"/>から一定間隔で呼ばれることを想定する。
	/// </summary>
	public void Sample()
	{
		if (!IsMonitoring || SelectedInterface is null)
		{
			return;
		}

		var (sent, received) = _sampler.Sample(SelectedInterface);
		_history.Add(new BandwidthSample(DateTime.Now, sent, received));
		RebuildPlotModel();
	}

	private bool CanStartMonitoring() => !IsMonitoring && SelectedInterface is not null;

	private void StartMonitoring() => IsMonitoring = true;

	private bool CanStopMonitoring() => IsMonitoring;

	private void StopMonitoring() => IsMonitoring = false;

	private void RebuildPlotModel() => PlotModel = BandwidthChartModelBuilder.Build(_history.Samples);
}
