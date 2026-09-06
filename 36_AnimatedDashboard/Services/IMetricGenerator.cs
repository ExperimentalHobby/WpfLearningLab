using AnimatedDashboard.Models;

namespace AnimatedDashboard.Services;

/// <summary>
/// KPI指標一覧を取得する処理の抽象。
/// </summary>
public interface IMetricGenerator
{
	/// <summary>
	/// KPI指標一覧を取得する。
	/// </summary>
	IReadOnlyList<KpiMetric> Generate();
}
