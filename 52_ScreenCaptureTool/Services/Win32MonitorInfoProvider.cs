using System.Runtime.InteropServices;
using System.Windows;
using ScreenCaptureTool.Models;

namespace ScreenCaptureTool.Services;

/// <summary>
/// Win32 API(<c>EnumDisplayMonitors</c>/<c>GetDpiForMonitor</c>)で接続中のモニタ情報を取得する実装。
/// </summary>
public class Win32MonitorInfoProvider : IMonitorInfoProvider
{
	private delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

	[StructLayout(LayoutKind.Sequential)]
	private struct RECT
	{
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;
	}

	[DllImport("user32.dll")]
	private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

	[DllImport("shcore.dll")]
	private static extern int GetDpiForMonitor(IntPtr hMonitor, int dpiType, out uint dpiX, out uint dpiY);

	private const int MdtEffectiveDpi = 0;

	/// <inheritdoc/>
	public IReadOnlyList<MonitorInfo> GetMonitors()
	{
		var monitors = new List<MonitorInfo>();

		EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (IntPtr hMonitor, IntPtr _, ref RECT rect, IntPtr _) =>
		{
			var scaleX = 1.0;
			var scaleY = 1.0;
			if (GetDpiForMonitor(hMonitor, MdtEffectiveDpi, out var dpiX, out var dpiY) == 0)
			{
				scaleX = dpiX / 96.0;
				scaleY = dpiY / 96.0;
			}

			var physicalWidth = rect.Right - rect.Left;
			var physicalHeight = rect.Bottom - rect.Top;
			var physicalBounds = new CaptureRegion(rect.Left, rect.Top, physicalWidth, physicalHeight);

			// このモニタ単体のDPIスケールで物理→論理変換した範囲を、このモニタの論理座標系とする簡易実装。
			// (モニタ間の論理座標オフセットの厳密な累積計算はWindowsの内部仕様に依存するため、
			// 本アプリでは単一モニタでの動作保証を優先し、この近似で座標変換ロジックを検証する)
			var logicalBounds = new Rect(
				rect.Left / scaleX, rect.Top / scaleY,
				physicalWidth / scaleX, physicalHeight / scaleY);

			monitors.Add(new MonitorInfo(logicalBounds, physicalBounds, scaleX, scaleY));
			return true;
		}, IntPtr.Zero);

		return monitors;
	}
}
