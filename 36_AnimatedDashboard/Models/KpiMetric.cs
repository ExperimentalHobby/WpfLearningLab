namespace AnimatedDashboard.Models;

/// <summary>
/// KPI風の指標1件。
/// </summary>
/// <param name="Name">指標名。</param>
/// <param name="Unit">単位。</param>
/// <param name="Value">値。</param>
public record KpiMetric(string Name, string Unit, double Value);
