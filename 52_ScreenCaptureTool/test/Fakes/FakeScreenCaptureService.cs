using System.Windows.Media;
using System.Windows.Media.Imaging;
using ScreenCaptureTool.Models;
using ScreenCaptureTool.Services;

namespace ScreenCaptureTool.Tests.Fakes;

/// <summary>
/// テスト用の<see cref="IScreenCaptureService"/>フェイク実装。実際のGDI呼び出しは行わず、
/// 呼び出し内容を記録した上でダミー画像を返す。
/// </summary>
public class FakeScreenCaptureService : IScreenCaptureService
{
	public int FullScreenCallCount { get; private set; }

	public CaptureRegion? LastRequestedRegion { get; private set; }

	public BitmapSource CaptureFullScreen()
	{
		FullScreenCallCount++;
		return CreateDummyImage();
	}

	public BitmapSource CaptureRegion(CaptureRegion region)
	{
		LastRequestedRegion = region;
		return CreateDummyImage();
	}

	private static BitmapSource CreateDummyImage() =>
		BitmapSource.Create(1, 1, 96, 96, PixelFormats.Bgra32, null, new byte[4], 4);
}
