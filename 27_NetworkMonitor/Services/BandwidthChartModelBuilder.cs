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
	public static PlotModel Build(IReadOnlyCollection<BandwidthSample> samples)
	{
		var model = new PlotModel { Title = "帯域使用状況 (bytes/sec)" };
		Rebuild(model, samples);
		return model;
	}

	/// <summary>
	/// 既存の<see cref="PlotModel"/>インスタンスの内容を、指定した履歴一覧に置き換える。
	/// 新しい<see cref="PlotModel"/>を作らないため、呼び出し元は更新後に
	/// <see cref="PlotModel.InvalidatePlot(bool)"/>を呼んで再描画をトリガーする必要がある。
	/// </summary>
	/// <param name="model">更新対象の<see cref="PlotModel"/>。</param>
	/// <param name="samples">帯域計測の履歴一覧(古い順)。</param>
	public static void Rebuild(PlotModel model, IReadOnlyCollection<BandwidthSample> samples)
	{
		model.Series.Clear();
		model.Axes.Clear();
		model.Legends.Clear();

		var sentSeries = new LineSeries { Title = "送信" };
		var receivedSeries = new LineSeries { Title = "受信" };
		var index = 0;
		foreach (var sample in samples)
		{
			sentSeries.Points.Add(new DataPoint(index, sample.SentBytesPerSec));
			receivedSeries.Points.Add(new DataPoint(index, sample.ReceivedBytesPerSec));
			index++;
		}

		model.Series.Add(sentSeries);
		model.Series.Add(receivedSeries);
		model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "サンプル" });
		model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "bytes/sec" });
		model.Legends.Add(new Legend { LegendPosition = LegendPosition.TopRight });
	}
}
