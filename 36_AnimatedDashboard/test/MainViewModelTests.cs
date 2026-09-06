using AnimatedDashboard.Models;
using AnimatedDashboard.ViewModels;

namespace AnimatedDashboard.Tests;

/// <summary>
/// <see cref="MainViewModel"/>のテスト。
/// </summary>
public class MainViewModelTests
{
	/// <summary>
	/// パス条件: コンストラクタで、ジェネレーターが返す指標一覧がMetricsに反映されること。
	/// </summary>
	[Fact]
	public void Constructor_ジェネレーターの指標一覧がMetricsに反映される()
	{
		var generator = new FakeMetricGenerator([new KpiMetric("売上", "万円", 100)]);

		var viewModel = new MainViewModel(generator);

		var metric = Assert.Single(viewModel.Metrics);
		Assert.Equal("売上", metric.Name);
		Assert.Equal(100, metric.Value);
	}

	/// <summary>
	/// パス条件: RefreshCommand実行で、件数が同じ場合は既存カードのValueが更新されること
	/// (カード自体は差し替えず、カウントアップアニメーションを維持する)。
	/// </summary>
	[Fact]
	public void RefreshCommand_件数が同じ場合既存カードのValueが更新される()
	{
		var generator = new FakeMetricGenerator(
			[new KpiMetric("売上", "万円", 100)],
			[new KpiMetric("売上", "万円", 200)]);
		var viewModel = new MainViewModel(generator);
		var originalCard = viewModel.Metrics[0];

		viewModel.RefreshCommand.Execute(null);

		Assert.Single(viewModel.Metrics);
		Assert.Same(originalCard, viewModel.Metrics[0]);
		Assert.Equal(200, viewModel.Metrics[0].Value);
	}

	/// <summary>
	/// パス条件: RefreshCommand実行で、新しい指標の件数が増えた場合、カードが追加され
	/// 既存分は取り残されず全件表示されること。
	/// </summary>
	[Fact]
	public void RefreshCommand_件数が増えた場合カードが追加される()
	{
		var generator = new FakeMetricGenerator(
			[new KpiMetric("売上", "万円", 100)],
			[new KpiMetric("売上", "万円", 150), new KpiMetric("ユーザー数", "人", 300)]);
		var viewModel = new MainViewModel(generator);

		viewModel.RefreshCommand.Execute(null);

		Assert.Equal(2, viewModel.Metrics.Count);
		Assert.Equal(150, viewModel.Metrics[0].Value);
		Assert.Equal("ユーザー数", viewModel.Metrics[1].Name);
		Assert.Equal(300, viewModel.Metrics[1].Value);
	}

	/// <summary>
	/// パス条件: RefreshCommand実行で、新しい指標の件数が減った場合、超過分のカードが
	/// 削除され表示上取り残されないこと。
	/// </summary>
	[Fact]
	public void RefreshCommand_件数が減った場合超過分のカードが削除される()
	{
		var generator = new FakeMetricGenerator(
			[new KpiMetric("売上", "万円", 100), new KpiMetric("ユーザー数", "人", 300)],
			[new KpiMetric("売上", "万円", 150)]);
		var viewModel = new MainViewModel(generator);

		viewModel.RefreshCommand.Execute(null);

		var metric = Assert.Single(viewModel.Metrics);
		Assert.Equal("売上", metric.Name);
		Assert.Equal(150, metric.Value);
	}
}
