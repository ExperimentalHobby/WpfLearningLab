using NetworkMonitor.Models;
using NetworkMonitor.Services;
using OxyPlot.Series;

namespace NetworkMonitor.Tests;

/// <summary>
/// <see cref="BandwidthChartModelBuilder"/> の単体テスト。
/// </summary>
public class BandwidthChartModelBuilderTests
{
	private static readonly IReadOnlyList<BandwidthSample> SampleHistory =
	[
		new(DateTime.Now, 100, 200),
		new(DateTime.Now, 150, 250),
		new(DateTime.Now, 120, 220),
	];

	/// <summary>
	/// パス条件: 履歴一覧から送信/受信の2系列のLineSeriesが生成され、それぞれサンプル数分のPointを持つこと
	/// </summary>
	[Fact]
	public void Build_送信受信2系列のLineSeriesがサンプル数分のPointを持つ()
	{
		var model = BandwidthChartModelBuilder.Build(SampleHistory);

		var lineSeriesList = model.Series.OfType<LineSeries>().ToList();
		Assert.Equal(2, lineSeriesList.Count);
		Assert.All(lineSeriesList, series => Assert.Equal(3, series.Points.Count));
	}

	/// <summary>
	/// パス条件: 送信系列のPointの値が、SentBytesPerSecの値と一致すること
	/// </summary>
	[Fact]
	public void Build_送信系列のPointの値がSentBytesPerSecと一致する()
	{
		var model = BandwidthChartModelBuilder.Build(SampleHistory);

		var sentSeries = model.Series.OfType<LineSeries>().Single(s => s.Title == "送信");
		Assert.Equal([100.0, 150.0, 120.0], sentSeries.Points.Select(p => p.Y));
	}

	/// <summary>
	/// パス条件: 受信系列のPointの値が、ReceivedBytesPerSecの値と一致すること
	/// </summary>
	[Fact]
	public void Build_受信系列のPointの値がReceivedBytesPerSecと一致する()
	{
		var model = BandwidthChartModelBuilder.Build(SampleHistory);

		var receivedSeries = model.Series.OfType<LineSeries>().Single(s => s.Title == "受信");
		Assert.Equal([200.0, 250.0, 220.0], receivedSeries.Points.Select(p => p.Y));
	}

	/// <summary>
	/// パス条件: 履歴が空の場合、送信/受信系列のPointが0件のPlotModelを返すこと
	/// </summary>
	[Fact]
	public void Build_履歴が空の場合Pointが0件のPlotModelを返す()
	{
		var model = BandwidthChartModelBuilder.Build([]);

		var lineSeriesList = model.Series.OfType<LineSeries>().ToList();
		Assert.Equal(2, lineSeriesList.Count);
		Assert.All(lineSeriesList, series => Assert.Empty(series.Points));
	}

	/// <summary>
	/// パス条件: Rebuildは新しいPlotModelを作らず、渡された既存インスタンスのSeriesを置き換えること
	/// </summary>
	[Fact]
	public void Rebuild_既存のPlotModelインスタンスを再利用してSeriesを置き換える()
	{
		var model = new OxyPlot.PlotModel();
		BandwidthChartModelBuilder.Rebuild(model, SampleHistory);
		Assert.Equal(3, model.Series.OfType<LineSeries>().First().Points.Count);

		BandwidthChartModelBuilder.Rebuild(model, [SampleHistory[0]]);

		Assert.Single(model.Series.OfType<LineSeries>().First().Points);
	}
}
