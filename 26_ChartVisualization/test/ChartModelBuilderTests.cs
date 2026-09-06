using ChartVisualization.Models;
using ChartVisualization.Services;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace ChartVisualization.Tests;

/// <summary>
/// <see cref="ChartModelBuilder"/> の単体テスト。
/// <see cref="OxyPlot.PlotModel"/> はUIに依存しないプレーンオブジェクトのため、
/// 実際に生成されたSeriesの型・件数・値を検証できる。
/// </summary>
public class ChartModelBuilderTests
{
	private static readonly IReadOnlyList<DataPoint> SampleData =
	[
		new("1月", 10),
		new("2月", 20),
		new("3月", 30),
	];

	/// <summary>
	/// パス条件: Bar種別の場合、データ件数分のBarItemを持つBarSeriesが1つ生成されること
	/// </summary>
	[Fact]
	public void Build_Bar種別の場合データ件数分のBarItemを持つBarSeriesが生成される()
	{
		var model = ChartModelBuilder.Build(SampleData, ChartType.Bar);

		var series = Assert.Single(model.Series.OfType<BarSeries>());
		Assert.Equal([10.0, 20.0, 30.0], series.Items.Select(item => item.Value));
	}

	/// <summary>
	/// パス条件: Bar種別の場合、ラベルがCategoryAxisに反映されること
	/// </summary>
	[Fact]
	public void Build_Bar種別の場合ラベルがCategoryAxisに反映される()
	{
		var model = ChartModelBuilder.Build(SampleData, ChartType.Bar);

		var axis = Assert.Single(model.Axes.OfType<CategoryAxis>());
		Assert.Equal(["1月", "2月", "3月"], axis.Labels);
	}

	/// <summary>
	/// パス条件: Line種別の場合、データ件数分のPointを持つLineSeriesが1つ生成されること
	/// </summary>
	[Fact]
	public void Build_Line種別の場合データ件数分のPointを持つLineSeriesが生成される()
	{
		var model = ChartModelBuilder.Build(SampleData, ChartType.Line);

		var series = Assert.Single(model.Series.OfType<LineSeries>());
		Assert.Equal([10.0, 20.0, 30.0], series.Points.Select(point => point.Y));
	}

	/// <summary>
	/// パス条件: Pie種別の場合、データ件数分のPieSliceを持つPieSeriesが1つ生成されること
	/// </summary>
	[Fact]
	public void Build_Pie種別の場合データ件数分のPieSliceを持つPieSeriesが生成される()
	{
		var model = ChartModelBuilder.Build(SampleData, ChartType.Pie);

		var series = Assert.Single(model.Series.OfType<PieSeries>());
		Assert.Equal(["1月", "2月", "3月"], series.Slices.Select(slice => slice.Label));
		Assert.Equal([10.0, 20.0, 30.0], series.Slices.Select(slice => slice.Value));
	}

	/// <summary>
	/// パス条件: データが空の場合、Series無しのPlotModelを返すこと
	/// </summary>
	[Fact]
	public void Build_データが空の場合Series無しのPlotModelを返す()
	{
		var model = ChartModelBuilder.Build([], ChartType.Bar);

		Assert.Empty(model.Series);
	}

	/// <summary>
	/// パス条件: Bar種別の場合、CategoryAxisがY軸(Left)に配置されること。
	/// OxyPlotの<c>BarSeries</c>はCategoryAxisがY軸に無いと実描画時に例外を投げるが、
	/// <see cref="OxyPlot.IPlotModel.Update"/>だけを呼んでもこの不整合は検出できず(実描画パスでのみ検証される)、
	/// 実際に<c>PlotView</c>で表示して初めて「BarSeries requires a CategoryAxis on the Y Axis.」という
	/// 例外が発生することを確認して見つけたバグ。そのため軸の配置を直接テストする。
	/// </summary>
	[Fact]
	public void Build_Bar種別の場合CategoryAxisがY軸に配置される()
	{
		var model = ChartModelBuilder.Build(SampleData, ChartType.Bar);

		var categoryAxis = Assert.Single(model.Axes.OfType<CategoryAxis>());
		Assert.Equal(AxisPosition.Left, categoryAxis.Position);
	}

	/// <summary>
	/// パス条件: Rebuildは新しいPlotModelを作らず、渡された既存インスタンスのSeries/Axesを
	/// 置き換えること(OxyPlotのInvalidatePlot()運用のため、Modelの参照自体は変えない)
	/// </summary>
	[Fact]
	public void Rebuild_既存のPlotModelインスタンスを再利用してSeriesを置き換える()
	{
		var model = new OxyPlot.PlotModel();
		ChartModelBuilder.Rebuild(model, SampleData, ChartType.Bar);
		Assert.Single(model.Series.OfType<BarSeries>());

		ChartModelBuilder.Rebuild(model, SampleData, ChartType.Pie);

		Assert.Empty(model.Series.OfType<BarSeries>());
		Assert.Empty(model.Axes);
		Assert.Single(model.Series.OfType<PieSeries>());
	}
}
