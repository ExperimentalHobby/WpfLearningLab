using ChartVisualization.Models;
using ChartVisualization.ViewModels;
using OxyPlot.Series;

namespace ChartVisualization.Tests;

/// <summary>
/// <see cref="MainViewModel"/> の単体テスト。
/// </summary>
public class MainViewModelTests
{
	/// <summary>
	/// パス条件: AddDataPointCommand実行でDataPointsに追加されPlotModelが再構築されること
	/// </summary>
	[Fact]
	public void AddDataPointCommand_実行するとDataPointsに追加されPlotModelが再構築される()
	{
		var viewModel = new MainViewModel();
		viewModel.NewLabel = "1月";
		viewModel.NewValueInput = "10";

		viewModel.AddDataPointCommand.Execute(null);

		Assert.Single(viewModel.DataPoints);
		Assert.Equal(new DataPoint("1月", 10), viewModel.DataPoints[0]);
		Assert.Single(viewModel.PlotModel.Series.OfType<BarSeries>());
	}

	/// <summary>
	/// パス条件: AddDataPointCommand実行後、入力欄がクリアされること
	/// </summary>
	[Fact]
	public void AddDataPointCommand_実行すると入力欄がクリアされる()
	{
		var viewModel = new MainViewModel();
		viewModel.NewLabel = "1月";
		viewModel.NewValueInput = "10";

		viewModel.AddDataPointCommand.Execute(null);

		Assert.Equal(string.Empty, viewModel.NewLabel);
		Assert.Equal(string.Empty, viewModel.NewValueInput);
	}

	/// <summary>
	/// パス条件: ラベルが空欄、または値が数値でない場合、AddDataPointCommandが実行不可になること
	/// </summary>
	[Theory]
	[InlineData("", "10")]
	[InlineData("1月", "")]
	[InlineData("1月", "abc")]
	public void AddDataPointCommand_ラベル空欄または値が数値でない場合CanExecuteがfalseになる(string label, string valueInput)
	{
		var viewModel = new MainViewModel();
		viewModel.NewLabel = label;
		viewModel.NewValueInput = valueInput;

		Assert.False(viewModel.AddDataPointCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: RemoveDataPointCommand実行でDataPointsから削除されPlotModelが再構築されること
	/// </summary>
	[Fact]
	public void RemoveDataPointCommand_実行するとDataPointsから削除されPlotModelが再構築される()
	{
		var viewModel = new MainViewModel();
		viewModel.NewLabel = "1月";
		viewModel.NewValueInput = "10";
		viewModel.AddDataPointCommand.Execute(null);
		var dataPoint = viewModel.DataPoints[0];

		viewModel.RemoveDataPointCommand.Execute(dataPoint);

		Assert.Empty(viewModel.DataPoints);
		Assert.Empty(viewModel.PlotModel.Series);
	}

	/// <summary>
	/// パス条件: SelectedChartTypeを変更すると、PlotModelの系列の型が切り替わること
	/// </summary>
	[Fact]
	public void SelectedChartType_変更するとPlotModelの系列の型が切り替わる()
	{
		var viewModel = new MainViewModel();
		viewModel.NewLabel = "1月";
		viewModel.NewValueInput = "10";
		viewModel.AddDataPointCommand.Execute(null);
		Assert.Single(viewModel.PlotModel.Series.OfType<BarSeries>());

		viewModel.SelectedChartType = ChartType.Pie;

		Assert.Single(viewModel.PlotModel.Series.OfType<PieSeries>());
		Assert.Empty(viewModel.PlotModel.Series.OfType<BarSeries>());
	}

	/// <summary>
	/// パス条件: 初期状態のSelectedChartTypeはBarであること
	/// </summary>
	[Fact]
	public void SelectedChartType_初期値はBarである()
	{
		var viewModel = new MainViewModel();

		Assert.Equal(ChartType.Bar, viewModel.SelectedChartType);
	}

	/// <summary>
	/// パス条件: 実行環境のカルチャが小数点にカンマを使う場合でも、"."区切りの入力値が
	/// 正しく解釈されデータ点が追加されること(カルチャに依存しない挙動になっていること)
	/// </summary>
	[Fact]
	public void AddDataPointCommand_カルチャに依存せず小数点付きの値を解釈できる()
	{
		var originalCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
		System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("de-DE");
		try
		{
			var viewModel = new MainViewModel();
			viewModel.NewLabel = "1月";
			viewModel.NewValueInput = "12.5";

			Assert.True(viewModel.AddDataPointCommand.CanExecute(null));
			viewModel.AddDataPointCommand.Execute(null);

			Assert.Equal(new DataPoint("1月", 12.5), viewModel.DataPoints[0]);
		}
		finally
		{
			System.Threading.Thread.CurrentThread.CurrentCulture = originalCulture;
		}
	}
}
