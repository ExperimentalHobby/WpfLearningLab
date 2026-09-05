using ScreenCaptureTool.Models;

namespace ScreenCaptureTool.Services;

/// <summary>
/// <see cref="SelectionOverlayWindow"/>を表示してマウスドラッグによる範囲選択を行う
/// <see cref="IRegionSelector"/>の実装。
/// </summary>
public class WpfRegionSelector(IMonitorInfoProvider monitorInfoProvider) : IRegionSelector
{
	/// <inheritdoc/>
	public CaptureRegion? SelectRegion()
	{
		var overlay = new SelectionOverlayWindow();
		var result = overlay.ShowDialog();

		if (result != true || overlay.SelectedLogicalRect is not { } logicalRect
			|| logicalRect.Width == 0 || logicalRect.Height == 0)
		{
			return null;
		}

		var monitors = monitorInfoProvider.GetMonitors();
		var physicalRegions = MonitorDpiCoordinateConverter.ToPhysicalRegions(logicalRect, monitors);
		if (physicalRegions.Count == 0)
		{
			return null;
		}

		// モニタ境界をまたぐ選択は、各モニタの担当領域を包含するバウンディングボックスとして結合する。
		var left = physicalRegions.Min(r => r.Left);
		var top = physicalRegions.Min(r => r.Top);
		var right = physicalRegions.Max(r => r.Left + r.Width);
		var bottom = physicalRegions.Max(r => r.Top + r.Height);

		return new CaptureRegion(left, top, right - left, bottom - top);
	}
}
