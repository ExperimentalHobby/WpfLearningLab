using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using ScreenCaptureTool.Models;

namespace ScreenCaptureTool.Services;

/// <summary>
/// GDI(<see cref="Graphics.CopyFromScreen"/>)による画面キャプチャの実装。
/// </summary>
public class GdiScreenCaptureService(IMonitorInfoProvider monitorInfoProvider) : IScreenCaptureService
{
	[DllImport("gdi32.dll")]
	private static extern bool DeleteObject(IntPtr hObject);

	/// <inheritdoc/>
	public BitmapSource CaptureFullScreen()
	{
		var monitors = monitorInfoProvider.GetMonitors();
		if (monitors.Count == 0)
		{
			throw new InvalidOperationException("接続中のモニタが検出できませんでした。");
		}

		var left = monitors.Min(m => m.PhysicalBounds.Left);
		var top = monitors.Min(m => m.PhysicalBounds.Top);
		var right = monitors.Max(m => m.PhysicalBounds.Left + m.PhysicalBounds.Width);
		var bottom = monitors.Max(m => m.PhysicalBounds.Top + m.PhysicalBounds.Height);

		return CaptureRegion(new CaptureRegion(left, top, right - left, bottom - top));
	}

	/// <inheritdoc/>
	public BitmapSource CaptureRegion(CaptureRegion region)
	{
		using var bitmap = new Bitmap(region.Width, region.Height);
		using (var graphics = Graphics.FromImage(bitmap))
		{
			graphics.CopyFromScreen(region.Left, region.Top, 0, 0, new System.Drawing.Size(region.Width, region.Height));
		}

		return ConvertToBitmapSource(bitmap);
	}

	private static BitmapSource ConvertToBitmapSource(Bitmap bitmap)
	{
		var hBitmap = bitmap.GetHbitmap();
		try
		{
			var source = Imaging.CreateBitmapSourceFromHBitmap(
				hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			source.Freeze();
			return source;
		}
		finally
		{
			DeleteObject(hBitmap);
		}
	}
}
