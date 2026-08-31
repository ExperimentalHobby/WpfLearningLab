using AnimatedDashboard.Models;
using AnimatedDashboard.Views;

namespace AnimatedDashboard.Tests;

/// <summary>
/// <see cref="KpiCard"/>のテスト。実際に<see cref="KpiCard"/>インスタンスを生成し検証する(STAスレッドが必要)。
/// </summary>
public class KpiCardTests
{
	/// <summary>
	/// パス条件: TargetValueを変更しても、Storyboardの構築・開始で例外が発生しないこと。
	/// </summary>
	[WpfFact]
	public void TargetValue変更_Storyboard開始で例外が発生しない()
	{
		var card = new KpiCard { Easing = EasingType.Bounce };

		var exception = Record.Exception(() => card.TargetValue = 123.4);

		Assert.Null(exception);
	}

	/// <summary>
	/// パス条件: TargetValueプロパティのgetterが設定した値をそのまま返すこと。
	/// </summary>
	[WpfFact]
	public void TargetValue_設定した値がそのまま取得できる()
	{
		var card = new KpiCard { TargetValue = 42 };

		Assert.Equal(42, card.TargetValue);
	}
}
