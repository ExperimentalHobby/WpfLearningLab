namespace ChartVisualization.Models;

/// <summary>
/// グラフに表示する1つのデータ点(ラベルと数値)。
/// </summary>
/// <param name="Label">X軸・凡例に表示するラベル。</param>
/// <param name="Value">数値。</param>
public record DataPoint(string Label, double Value);
