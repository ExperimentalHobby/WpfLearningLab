namespace ExchangeRateMonitor.ViewModels;

/// <summary>
/// 前回レートと比較した際の変動方向。
/// </summary>
public enum RateTrend
{
	/// <summary>前回値が無く比較できない(初回取得時)。</summary>
	Unknown,

	/// <summary>前回より上昇。</summary>
	Up,

	/// <summary>前回より下落。</summary>
	Down,

	/// <summary>前回と変わらず。</summary>
	Unchanged,
}
