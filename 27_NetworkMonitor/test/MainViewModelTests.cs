using NetworkMonitor.ViewModels;
using OxyPlot.Series;

namespace NetworkMonitor.Tests;

/// <summary>
/// <see cref="MainViewModel"/> の単体テスト。
/// </summary>
public class MainViewModelTests
{
	private static MainViewModel CreateViewModel(FakeNetworkBandwidthSampler? sampler = null, int maxSampleCount = 60) =>
		new(sampler ?? new FakeNetworkBandwidthSampler(), maxSampleCount);

	/// <summary>
	/// パス条件: コンストラクタでNetworkInterfacesがsamplerの返すインターフェース名一覧になること
	/// </summary>
	[Fact]
	public void コンストラクタ_NetworkInterfacesがsamplerの一覧になる()
	{
		var sampler = new FakeNetworkBandwidthSampler { InstanceNames = ["eth0", "wlan0"] };

		var viewModel = CreateViewModel(sampler);

		Assert.Equal(["eth0", "wlan0"], viewModel.NetworkInterfaces);
	}

	/// <summary>
	/// パス条件: SelectedInterface未選択の場合、StartMonitoringCommandが実行不可になること
	/// </summary>
	[Fact]
	public void StartMonitoringCommand_SelectedInterface未選択の場合CanExecuteがfalseになる()
	{
		var viewModel = CreateViewModel();

		Assert.False(viewModel.StartMonitoringCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: StartMonitoringCommand実行でIsMonitoringがtrueになること
	/// </summary>
	[Fact]
	public void StartMonitoringCommand_実行するとIsMonitoringがtrueになる()
	{
		var viewModel = CreateViewModel();
		viewModel.SelectedInterface = "eth0";

		viewModel.StartMonitoringCommand.Execute(null);

		Assert.True(viewModel.IsMonitoring);
	}

	/// <summary>
	/// パス条件: StopMonitoringCommand実行でIsMonitoringがfalseになること
	/// </summary>
	[Fact]
	public void StopMonitoringCommand_実行するとIsMonitoringがfalseになる()
	{
		var viewModel = CreateViewModel();
		viewModel.SelectedInterface = "eth0";
		viewModel.StartMonitoringCommand.Execute(null);

		viewModel.StopMonitoringCommand.Execute(null);

		Assert.False(viewModel.IsMonitoring);
	}

	/// <summary>
	/// パス条件: 監視中にSample()を呼ぶと履歴に追加されPlotModelが更新されること
	/// </summary>
	[Fact]
	public void Sample_監視中に呼ぶと履歴に追加されPlotModelが更新される()
	{
		var sampler = new FakeNetworkBandwidthSampler();
		var viewModel = CreateViewModel(sampler);
		viewModel.SelectedInterface = "eth0";
		viewModel.StartMonitoringCommand.Execute(null);

		viewModel.Sample();

		var sentSeries = viewModel.PlotModel.Series.OfType<LineSeries>().Single(s => s.Title == "送信");
		Assert.Single(sentSeries.Points);
		Assert.Equal(1, sampler.SampleCallCount);
	}

	/// <summary>
	/// パス条件: 監視中でない場合にSample()を呼んでも何もしないこと
	/// </summary>
	[Fact]
	public void Sample_監視中でない場合何もしない()
	{
		var sampler = new FakeNetworkBandwidthSampler();
		var viewModel = CreateViewModel(sampler);
		viewModel.SelectedInterface = "eth0";

		viewModel.Sample();

		Assert.Equal(0, sampler.SampleCallCount);
	}

	/// <summary>
	/// パス条件: SelectedInterfaceを変更すると、履歴がクリアされること
	/// </summary>
	[Fact]
	public void SelectedInterface_変更すると履歴がクリアされる()
	{
		var sampler = new FakeNetworkBandwidthSampler();
		var viewModel = CreateViewModel(sampler);
		viewModel.SelectedInterface = "eth0";
		viewModel.StartMonitoringCommand.Execute(null);
		viewModel.Sample();

		viewModel.SelectedInterface = "wlan0";

		var sentSeries = viewModel.PlotModel.Series.OfType<LineSeries>().Single(s => s.Title == "送信");
		Assert.Empty(sentSeries.Points);
	}
}
