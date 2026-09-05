using System.Windows;
using ScreenCaptureTool.Models;
using ScreenCaptureTool.Services;

namespace ScreenCaptureTool.Tests;

public class MonitorDpiCoordinateConverterTests
{
	/// <summary>
	/// パス条件: 開始点が終了点より左上にある通常のドラッグで、そのままの矩形が求まること。
	/// </summary>
	[Fact]
	public void Normalize_ForwardDrag_ReturnsExpectedRect()
	{
		var rect = MonitorDpiCoordinateConverter.Normalize(new Point(10, 20), new Point(110, 220));

		Assert.Equal(new Rect(10, 20, 100, 200), rect);
	}

	/// <summary>
	/// パス条件: 開始点が終了点より右下にある逆方向ドラッグでも、正の矩形が求まること。
	/// </summary>
	[Fact]
	public void Normalize_BackwardDrag_ReturnsNormalizedRect()
	{
		var rect = MonitorDpiCoordinateConverter.Normalize(new Point(110, 220), new Point(10, 20));

		Assert.Equal(new Rect(10, 20, 100, 200), rect);
	}

	/// <summary>
	/// パス条件: 開始点と終了点が同一の場合、幅・高さ0の矩形になること。
	/// </summary>
	[Fact]
	public void Normalize_SamePoint_ReturnsZeroSizeRect()
	{
		var rect = MonitorDpiCoordinateConverter.Normalize(new Point(50, 50), new Point(50, 50));

		Assert.Equal(new Rect(50, 50, 0, 0), rect);
	}

	/// <summary>
	/// パス条件: DPI100%のモニタでは論理座標がそのまま物理座標になること。
	/// </summary>
	[Fact]
	public void ToPhysicalRegion_Dpi100Percent_LogicalEqualsPhysical()
	{
		var monitor = new MonitorInfo(
			LogicalBounds: new Rect(0, 0, 1920, 1080),
			PhysicalBounds: new CaptureRegion(0, 0, 1920, 1080),
			DpiScaleX: 1.0,
			DpiScaleY: 1.0);

		var region = MonitorDpiCoordinateConverter.ToPhysicalRegion(new Rect(10, 20, 100, 200), monitor);

		Assert.Equal(new CaptureRegion(10, 20, 100, 200), region);
	}

	/// <summary>
	/// パス条件: DPI150%のモニタでは論理座標が1.5倍された物理座標になること。
	/// </summary>
	[Fact]
	public void ToPhysicalRegion_Dpi150Percent_ScalesCoordinates()
	{
		var monitor = new MonitorInfo(
			LogicalBounds: new Rect(0, 0, 1280, 720),
			PhysicalBounds: new CaptureRegion(0, 0, 1920, 1080),
			DpiScaleX: 1.5,
			DpiScaleY: 1.5);

		var region = MonitorDpiCoordinateConverter.ToPhysicalRegion(new Rect(10, 20, 100, 200), monitor);

		Assert.Equal(new CaptureRegion(15, 30, 150, 300), region);
	}

	/// <summary>
	/// パス条件: モニタが仮想スクリーン内で原点以外に配置されている(マルチモニタ)場合、
	/// 物理オフセットが論理座標からの相対位置に対して正しく反映されること。
	/// </summary>
	[Fact]
	public void ToPhysicalRegion_MonitorWithPhysicalOffset_AppliesOffset()
	{
		// セカンドモニタが仮想スクリーン上でX=1920(論理)から始まり、DPI200%で配置されている想定
		var monitor = new MonitorInfo(
			LogicalBounds: new Rect(1920, 0, 960, 540),
			PhysicalBounds: new CaptureRegion(1920, 0, 1920, 1080),
			DpiScaleX: 2.0,
			DpiScaleY: 2.0);

		// 論理座標(1930, 10)はモニタ原点(1920, 0)から見て相対(10, 10)
		var region = MonitorDpiCoordinateConverter.ToPhysicalRegion(new Rect(1930, 10, 50, 60), monitor);

		Assert.Equal(new CaptureRegion(1920 + 20, 0 + 20, 100, 120), region);
	}

	/// <summary>
	/// パス条件: モニタ境界をまたぐ矩形は、各モニタの担当領域(交差部分)ごとに物理座標へ変換されること。
	/// </summary>
	[Fact]
	public void ToPhysicalRegions_RectSpanningMultipleMonitors_SplitsPerMonitor()
	{
		var primary = new MonitorInfo(
			LogicalBounds: new Rect(0, 0, 1920, 1080),
			PhysicalBounds: new CaptureRegion(0, 0, 1920, 1080),
			DpiScaleX: 1.0,
			DpiScaleY: 1.0);
		var secondary = new MonitorInfo(
			LogicalBounds: new Rect(1920, 0, 960, 540),
			PhysicalBounds: new CaptureRegion(1920, 0, 1920, 1080),
			DpiScaleX: 2.0,
			DpiScaleY: 2.0);
		var monitors = new[] { primary, secondary };

		// 論理座標でプライマリモニタ側に1800〜、セカンダリモニタ側に〜1970 とまたがる矩形
		var regions = MonitorDpiCoordinateConverter.ToPhysicalRegions(new Rect(1800, 0, 170, 100), monitors);

		Assert.Equal(2, regions.Count);
		Assert.Contains(new CaptureRegion(1800, 0, 120, 100), regions);   // プライマリ側(DPI100%): 1800〜1920
		Assert.Contains(new CaptureRegion(1920, 0, 100, 200), regions);   // セカンダリ側(DPI200%): 相対0〜50が物理0〜100
	}

	/// <summary>
	/// パス条件: どのモニタとも重ならない矩形は空の結果になること。
	/// </summary>
	[Fact]
	public void ToPhysicalRegions_RectOutsideAllMonitors_ReturnsEmpty()
	{
		var monitor = new MonitorInfo(
			LogicalBounds: new Rect(0, 0, 1920, 1080),
			PhysicalBounds: new CaptureRegion(0, 0, 1920, 1080),
			DpiScaleX: 1.0,
			DpiScaleY: 1.0);

		var regions = MonitorDpiCoordinateConverter.ToPhysicalRegions(new Rect(5000, 5000, 10, 10), [monitor]);

		Assert.Empty(regions);
	}
}
