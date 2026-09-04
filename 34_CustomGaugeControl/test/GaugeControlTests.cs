using System.Windows;
using GaugeControlLib;

namespace GaugeControlLib.Tests;

/// <summary>
/// <see cref="GaugeControl"/>の<see cref="GaugeControl.ThresholdExceeded"/>ルーテッドイベントのテスト。
/// 実際に<see cref="GaugeControl"/>インスタンスを生成し、Valueプロパティの変更を通じて検証する。
/// </summary>
public class GaugeControlTests
{
	/// <summary>
	/// パス条件: Valueが下から上へThresholdを超えると、ThresholdExceededイベントが発火すること。
	/// </summary>
	[WpfFact]
	public void Value変更_ThresholdをThresholdをしきい値未満から超えるとThresholdExceededイベントが発火する()
	{
		var gauge = new GaugeControl { Minimum = 0, Maximum = 100, Threshold = 80, Value = 50 };
		var raised = false;
		gauge.ThresholdExceeded += (_, _) => raised = true;

		gauge.Value = 90;

		Assert.True(raised);
	}

	/// <summary>
	/// パス条件: Thresholdが未満のまま変化した場合、ThresholdExceededイベントは発火しないこと。
	/// </summary>
	[WpfFact]
	public void Value変更_しきい値未満のままの場合ThresholdExceededイベントは発火しない()
	{
		var gauge = new GaugeControl { Minimum = 0, Maximum = 100, Threshold = 80, Value = 10 };
		var raised = false;
		gauge.ThresholdExceeded += (_, _) => raised = true;

		gauge.Value = 20;

		Assert.False(raised);
	}

	/// <summary>
	/// パス条件: Thresholdが未設定(NaN)の場合、Valueが変化してもThresholdExceededイベントは発火しないこと。
	/// </summary>
	[WpfFact]
	public void Value変更_Threshold未設定の場合ThresholdExceededイベントは発火しない()
	{
		var gauge = new GaugeControl { Minimum = 0, Maximum = 100, Value = 10 };
		var raised = false;
		gauge.ThresholdExceeded += (_, _) => raised = true;

		gauge.Value = 100;

		Assert.False(raised);
	}
}
