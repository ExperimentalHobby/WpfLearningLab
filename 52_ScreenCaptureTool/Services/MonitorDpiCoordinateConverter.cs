using System.Windows;
using ScreenCaptureTool.Models;

namespace ScreenCaptureTool.Services;

/// <summary>
/// マウスドラッグで得た論理座標(DIU)を、モニタごとのDPIスケール・物理オフセットを考慮して
/// 物理ピクセル座標のキャプチャ範囲に変換する。UI非依存の純粋ロジック。
/// </summary>
public static class MonitorDpiCoordinateConverter
{
	/// <summary>
	/// ドラッグの開始点・終了点(どちらの方向でもよい)から、正規化された論理矩形を求める。
	/// </summary>
	/// <param name="start">ドラッグ開始点。</param>
	/// <param name="end">ドラッグ終了点。</param>
	public static Rect Normalize(Point start, Point end)
	{
		var x = Math.Min(start.X, end.X);
		var y = Math.Min(start.Y, end.Y);
		var width = Math.Abs(end.X - start.X);
		var height = Math.Abs(end.Y - start.Y);
		return new Rect(x, y, width, height);
	}

	/// <summary>
	/// 論理矩形(単一モニタの担当領域に収まっている前提)を、そのモニタのDPIスケール・物理オフセットを
	/// 使って物理ピクセル座標のキャプチャ範囲に変換する。
	/// </summary>
	/// <param name="logicalRect">変換対象の論理矩形。</param>
	/// <param name="monitor">対象モニタの座標変換情報。</param>
	public static CaptureRegion ToPhysicalRegion(Rect logicalRect, MonitorInfo monitor)
	{
		var relativeLeft = logicalRect.Left - monitor.LogicalBounds.Left;
		var relativeTop = logicalRect.Top - monitor.LogicalBounds.Top;

		var physicalLeft = monitor.PhysicalBounds.Left + (int)Math.Round(relativeLeft * monitor.DpiScaleX);
		var physicalTop = monitor.PhysicalBounds.Top + (int)Math.Round(relativeTop * monitor.DpiScaleY);
		var physicalWidth = (int)Math.Round(logicalRect.Width * monitor.DpiScaleX);
		var physicalHeight = (int)Math.Round(logicalRect.Height * monitor.DpiScaleY);

		return new CaptureRegion(physicalLeft, physicalTop, physicalWidth, physicalHeight);
	}

	/// <summary>
	/// 論理矩形を、重なりのある各モニタの担当領域(交差部分)ごとに物理ピクセル座標へ変換する。
	/// モニタ境界をまたぐ矩形は、複数のキャプチャ範囲として返る。
	/// </summary>
	/// <param name="logicalRect">変換対象の論理矩形。</param>
	/// <param name="monitors">候補となるモニタの一覧。</param>
	public static IReadOnlyList<CaptureRegion> ToPhysicalRegions(Rect logicalRect, IReadOnlyList<MonitorInfo> monitors)
	{
		var regions = new List<CaptureRegion>();

		foreach (var monitor in monitors)
		{
			var intersection = Rect.Intersect(logicalRect, monitor.LogicalBounds);
			if (intersection.IsEmpty)
			{
				continue;
			}

			regions.Add(ToPhysicalRegion(intersection, monitor));
		}

		return regions;
	}
}
