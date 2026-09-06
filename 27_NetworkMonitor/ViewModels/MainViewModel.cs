using System.ComponentModel;
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
	private string _errorMessage = string.Empty;

	// OxyPlotの推奨運用に従い、PlotModelインスタンス自体は差し替えずに保持し、内容が変わったら
	// Rebuild + InvalidatePlot(true)で再描画をトリガーする。
	private readonly PlotModel _plotModel = new() { Title = "帯域使用状況 (bytes/sec)" };

	/// <summary>
	/// ViewModelを初期化する。
	/// </summary>
	/// <param name="sampler">帯域計測処理。</param>
	/// <param name="maxSampleCount">保持する最大サンプル数(既定60件)。</param>
	public MainViewModel(INetworkBandwidthSampler sampler, int maxSampleCount = 60)
	{
		_sampler = sampler;
		_history = new BandwidthHistory(maxSampleCount);
		StartMonitoringCommand = new RelayCommand(StartMonitoring, CanStartMonitoring);
		StopMonitoringCommand = new RelayCommand(StopMonitoring, CanStopMonitoring);

		try
		{
			NetworkInterfaces = sampler.GetInstanceNames();
		}
		catch (Exception ex) when (ex is InvalidOperationException or UnauthorizedAccessException or Win32Exception)
		{
			// カウンターカテゴリの破損・権限不足等でGetInstanceNamesが例外を投げても、
			// アプリ自体は起動できるようにする(インターフェース一覧が空になるだけに留める)。
			NetworkInterfaces = [];
			ErrorMessage = $"ネットワークインターフェース一覧の取得に失敗しました: {ex.Message}";
		}

		RebuildPlotModel();
	}

	/// <summary>計測可能なネットワークインターフェースのインスタンス名一覧。</summary>
	public IReadOnlyList<string> NetworkInterfaces { get; }

	/// <summary>直近の操作で発生したエラーメッセージ。エラーがなければ空文字列。</summary>
	public string ErrorMessage
	{
		get => _errorMessage;
		private set => SetProperty(ref _errorMessage, value);
	}

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

	/// <summary>現在の履歴から組み立てられた<see cref="PlotModel"/>。インスタンスは固定で、内容のみ更新される。</summary>
	public PlotModel PlotModel => _plotModel;

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

		try
		{
			var (sent, received) = _sampler.Sample(SelectedInterface);
			_history.Add(new BandwidthSample(DateTime.Now, sent, received));
			RebuildPlotModel();
			ErrorMessage = string.Empty;
		}
		catch (Exception ex) when (ex is InvalidOperationException or UnauthorizedAccessException or Win32Exception)
		{
			// USB LANアダプタの取り外し等でインターフェースが消えると計測のたびに例外が発生し続ける
			// ため、監視を止めてユーザーに分かる形でエラーを表示する。
			ErrorMessage = $"計測に失敗しました: {ex.Message}";
			StopMonitoring();
		}
	}

	private bool CanStartMonitoring() => !IsMonitoring && SelectedInterface is not null;

	private void StartMonitoring() => IsMonitoring = true;

	private bool CanStopMonitoring() => IsMonitoring;

	private void StopMonitoring() => IsMonitoring = false;

	private void RebuildPlotModel()
	{
		BandwidthChartModelBuilder.Rebuild(_plotModel, _history.Samples);
		_plotModel.InvalidatePlot(true);
	}
}
