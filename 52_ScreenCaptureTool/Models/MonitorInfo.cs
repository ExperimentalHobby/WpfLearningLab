using System.Windows;

namespace ScreenCaptureTool.Models;

/// <summary>
/// 1台のモニタに関する座標変換情報。
/// </summary>
/// <param name="LogicalBounds">仮想スクリーンの論理座標系(DIU)でのこのモニタの範囲。</param>
/// <param name="PhysicalBounds">仮想デスクトップの物理ピクセル座標系でのこのモニタの範囲。</param>
/// <param name="DpiScaleX">このモニタの水平方向DPIスケール(100%なら1.0)。</param>
/// <param name="DpiScaleY">このモニタの垂直方向DPIスケール(100%なら1.0)。</param>
public record MonitorInfo(Rect LogicalBounds, CaptureRegion PhysicalBounds, double DpiScaleX, double DpiScaleY);
