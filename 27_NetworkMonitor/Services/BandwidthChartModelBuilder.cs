using NetworkMonitor.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;

namespace NetworkMonitor.Services;

/// <summary>
/// 帯域計測の履歴一覧から<see cref="PlotModel"/>を組み立てる純粋ロジック。
/// <see cref="PlotModel"/>はUIに依存しないプレーンオブジェクトのため、UIなしで単体テストできる。
/// </summary>
public static class BandwidthChartModelBuilder
{
	/// <summary>
	/// 帯域計測の履歴一覧から、送信/受信2系列の折れ線グラフを持つ<see cref="PlotModel"/>を組み立てる。
	/// </summary>
	/// <param name="samples">帯域計測の履歴一覧(古い順)。</param>
	public static PlotModel Build(IReadOnlyList<BandwidthSample> samples)
	{
		var model = new PlotModel { Title = "帯域使用状況 (bytes/sec)" };

		var sentSeries = new LineSeries { Title = "送信" };
		var receivedSeries = new LineSeries { Title = "受信" };
		for (var i = 0; i < samples.Count; i++)
		{
			sentSeries.Points.Add(new DataPoint(i, samples[i].SentBytesPerSec));
			receivedSeries.Points.Add(new DataPoint(i, samples[i].ReceivedBytesPerSec));
		}

		model.Series.Add(sentSeries);
		model.Series.Add(receivedSeries);
		model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "サンプル" });
		model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "bytes/sec" });
		model.Legends.Add(new Legend { LegendPosition = LegendPosition.TopRight });

		return model;
	}
}
