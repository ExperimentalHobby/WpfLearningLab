using ExchangeRateMonitor.ViewModels;

namespace ExchangeRateMonitor.Tests;

/// <summary>
/// <see cref="WatchedPairItem"/> の単体テスト。
/// </summary>
public class WatchedPairItemTests
{
	/// <summary>
	/// パス条件: 初回のレート更新ではTrendがUnknownになること
	/// </summary>
	[Fact]
	public void UpdateRate_初回はTrendがUnknownになる()
	{
		var item = new WatchedPairItem("USD", "JPY");

		item.UpdateRate(150.00m);

		Assert.Equal(RateTrend.Unknown, item.Trend);
		Assert.Equal(150.00m, item.CurrentRate);
	}

	/// <summary>
	/// パス条件: 前回より高いレートで更新するとTrendがUpになること
	/// </summary>
	[Fact]
	public void UpdateRate_前回より上昇するとTrendがUpになる()
	{
		var item = new WatchedPairItem("USD", "JPY");
		item.UpdateRate(150.00m);

		item.UpdateRate(151.00m);

		Assert.Equal(RateTrend.Up, item.Trend);
		Assert.Equal(150.00m, item.PreviousRate);
		Assert.Equal(151.00m, item.CurrentRate);
	}

	/// <summary>
	/// パス条件: 前回より低いレートで更新するとTrendがDownになること
	/// </summary>
	[Fact]
	public void UpdateRate_前回より下落するとTrendがDownになる()
	{
		var item = new WatchedPairItem("USD", "JPY");
		item.UpdateRate(150.00m);

		item.UpdateRate(149.00m);

		Assert.Equal(RateTrend.Down, item.Trend);
	}

	/// <summary>
	/// パス条件: 前回と同じレートで更新するとTrendがUnchangedになること
	/// </summary>
	[Fact]
	public void UpdateRate_前回と同じ場合Trendがunchangedになる()
	{
		var item = new WatchedPairItem("USD", "JPY");
		item.UpdateRate(150.00m);

		item.UpdateRate(150.00m);

		Assert.Equal(RateTrend.Unchanged, item.Trend);
	}
}
