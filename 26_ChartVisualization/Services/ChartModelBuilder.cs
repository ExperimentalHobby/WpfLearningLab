using ChartVisualization.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using DataPoint = ChartVisualization.Models.DataPoint;

namespace ChartVisualization.Services;

/// <summary>
/// データ点一覧とグラフ種類から<see cref="PlotModel"/>を組み立てる純粋ロジック。
/// <see cref="PlotModel"/>はUIに依存しないプレーンオブジェクトのため、UIなしで単体テストできる。
/// </summary>
public static class ChartModelBuilder
{
	/// <summary>
	/// データ点一覧とグラフ種類から<see cref="PlotModel"/>を組み立てる。
	/// </summary>
	/// <param name="dataPoints">グラフに表示するデータ点一覧。</param>
	/// <param name="chartType">グラフの種類。</param>
	public static PlotModel Build(IReadOnlyList<DataPoint> dataPoints, ChartType chartType)
	{
		var model = new PlotModel { Title = "データ可視化" };
		Rebuild(model, dataPoints, chartType);
		return model;
	}

	/// <summary>
	/// 既存の<see cref="PlotModel"/>インスタンスの<c>Series</c>/<c>Axes</c>を、指定したデータ点・
	/// グラフ種類の内容に置き換える。新しい<see cref="PlotModel"/>を作らないため、呼び出し元は
	/// 更新後に<see cref="PlotModel.InvalidatePlot(bool)"/>を呼んで再描画をトリガーする必要がある。
	/// </summary>
	/// <param name="model">更新対象の<see cref="PlotModel"/>。</param>
	/// <param name="dataPoints">グラフに表示するデータ点一覧。</param>
	/// <param name="chartType">グラフの種類。</param>
	public static void Rebuild(PlotModel model, IReadOnlyList<DataPoint> dataPoints, ChartType chartType)
	{
		model.Series.Clear();
		model.Axes.Clear();

		if (dataPoints.Count == 0)
		{
			return;
		}

		switch (chartType)
		{
			case ChartType.Bar:
				BuildBar(model, dataPoints);
				break;
			case ChartType.Line:
				BuildLine(model, dataPoints);
				break;
			case ChartType.Pie:
				BuildPie(model, dataPoints);
				break;
		}
	}

	private static void BuildBar(PlotModel model, IReadOnlyList<DataPoint> dataPoints)
	{
		// OxyPlotのBarSeriesは横棒グラフとして描画され、CategoryAxisをY軸(Left)に置く必要がある。
		// (X軸=Bottomに置くと、実際にPlotViewで描画する際に
		//  "BarSeries requires a CategoryAxis on the Y Axis." 例外が発生する。
		//  SeriesやAxesの型・件数だけを見る単体テストでは検出できず、実機でのUI確認で見つかった)
		var series = new BarSeries();
		foreach (var point in dataPoints)
		{
			series.Items.Add(new BarItem(point.Value));
		}

		model.Series.Add(series);
		model.Axes.Add(CreateCategoryAxis(dataPoints, AxisPosition.Left));
		model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });
	}

	private static void BuildLine(PlotModel model, IReadOnlyList<DataPoint> dataPoints)
	{
		var series = new LineSeries();
		for (var i = 0; i < dataPoints.Count; i++)
		{
			series.Points.Add(new OxyPlot.DataPoint(i, dataPoints[i].Value));
		}

		model.Series.Add(series);
		model.Axes.Add(CreateCategoryAxis(dataPoints, AxisPosition.Bottom));
		model.Axes.Add(new LinearAxis { Position = AxisPosition.Left });
	}

	private static void BuildPie(PlotModel model, IReadOnlyList<DataPoint> dataPoints)
	{
		var series = new PieSeries();
		foreach (var point in dataPoints)
		{
			series.Slices.Add(new PieSlice(point.Label, point.Value));
		}

		model.Series.Add(series);
	}

	private static CategoryAxis CreateCategoryAxis(IReadOnlyList<DataPoint> dataPoints, AxisPosition position)
	{
		var axis = new CategoryAxis { Position = position };
		foreach (var point in dataPoints)
		{
			axis.Labels.Add(point.Label);
		}

		return axis;
	}
}
