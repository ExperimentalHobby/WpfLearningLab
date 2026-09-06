using System.Windows;
using System.Windows.Threading;
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

	/// <summary>
	/// パス条件: MinimumにMaximumを超える値を設定すると、Maximumにクランプされること。
	/// </summary>
	[WpfFact]
	public void Minimum_Maximumを超える値を設定するとMaximumにクランプされる()
	{
		var gauge = new GaugeControl { Minimum = 0, Maximum = 100 };

		gauge.Minimum = 200;

		Assert.Equal(100, gauge.Minimum);
	}

	/// <summary>
	/// パス条件: MaximumにMinimum未満の値を設定すると、Minimumにクランプされること。
	/// </summary>
	[WpfFact]
	public void Maximum_Minimum未満の値を設定するとMinimumにクランプされる()
	{
		var gauge = new GaugeControl { Minimum = 0, Maximum = 100 };

		gauge.Maximum = -50;

		Assert.Equal(0, gauge.Maximum);
	}

	/// <summary>
	/// パス条件: Valueが同じままMinimum/Maximumが変わると、AnimatedAngleが新しい範囲に基づく
	/// 角度へ再計算されること。
	/// </summary>
	[WpfFact]
	public void Minimum変更_値に対する角度が再計算される()
	{
		var gauge = new GaugeControl { Minimum = 0, Maximum = 100, Value = 50 };
		// WPFのアニメーションクロックは実際の描画対象(HwndTarget)が無いと進行しないため、
		// 非表示でもWindowにアタッチしてレンダリングパイプラインに乗せる。
		var window = new Window { Content = gauge, ShowInTaskbar = false, WindowStyle = WindowStyle.None, Width = 1, Height = 1 };
		window.Show();
		try
		{
			WaitAndPumpDispatcher(600);

			gauge.Minimum = -100;
			WaitAndPumpDispatcher(600);

			var expectedAngle = GaugeMath.ValueToAngle(50, -100, 100);
			Assert.Equal(expectedAngle, gauge.AnimatedAngle, precision: 0);
		}
		finally
		{
			window.Close();
		}
	}

	/// <summary>
	/// アニメーション(400ms)の収束を待つため、指定時間だけDispatcherの保留メッセージ
	/// (レンダリングによるアニメーション更新を含む)を処理し続ける。
	/// </summary>
	private static void WaitAndPumpDispatcher(int milliseconds)
	{
		var endTime = DateTime.UtcNow.AddMilliseconds(milliseconds);
		while (DateTime.UtcNow < endTime)
		{
			var frame = new DispatcherFrame();
			Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => frame.Continue = false));
			Dispatcher.PushFrame(frame);
		}
	}
}
