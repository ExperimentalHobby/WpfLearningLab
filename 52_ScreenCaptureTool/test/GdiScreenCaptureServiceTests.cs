using ScreenCaptureTool.Models;
using ScreenCaptureTool.Services;
using ScreenCaptureTool.Tests.Fakes;

namespace ScreenCaptureTool.Tests;

public class GdiScreenCaptureServiceTests
{
	/// <summary>
	/// パス条件: Widthが0以下の範囲を指定すると、GDI呼び出し前にScreenCaptureExceptionが飛ぶこと。
	/// </summary>
	[Fact]
	public void CaptureRegion_WidthZeroOrLess_ThrowsScreenCaptureException()
	{
		var service = new GdiScreenCaptureService(new FakeMonitorInfoProvider());
		var region = new CaptureRegion(0, 0, 0, 100);

		Assert.Throws<ScreenCaptureException>(() => service.CaptureRegion(region));
	}

	/// <summary>
	/// パス条件: Heightが0以下の範囲を指定すると、GDI呼び出し前にScreenCaptureExceptionが飛ぶこと。
	/// </summary>
	[Fact]
	public void CaptureRegion_HeightZeroOrLess_ThrowsScreenCaptureException()
	{
		var service = new GdiScreenCaptureService(new FakeMonitorInfoProvider());
		var region = new CaptureRegion(0, 0, 100, 0);

		Assert.Throws<ScreenCaptureException>(() => service.CaptureRegion(region));
	}
}
