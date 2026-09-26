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
	/// <summary><see cref="CaptureFullScreen"/>が呼ばれた回数(テスト用)。</summary>
	public int FullScreenCallCount { get; private set; }

	/// <summary>直近の領域指定キャプチャ呼び出し引数(テスト用)。</summary>
	public CaptureRegion? LastRequestedRegion { get; private set; }

	/// <summary>
	/// 設定すると、次回の呼び出しでこの例外をスローする(異常系のテスト用)。
	/// </summary>
	public Exception? ExceptionToThrow { get; set; }

	/// <inheritdoc/>
	public BitmapSource CaptureFullScreen()
	{
		FullScreenCallCount++;
		if (ExceptionToThrow is not null)
		{
			throw ExceptionToThrow;
		}

		return CreateDummyImage();
	}

	/// <inheritdoc/>
	public BitmapSource CaptureRegion(CaptureRegion region)
	{
		LastRequestedRegion = region;
		if (ExceptionToThrow is not null)
		{
			throw ExceptionToThrow;
		}

		return CreateDummyImage();
	}

	private static BitmapSource CreateDummyImage() =>
		BitmapSource.Create(1, 1, 96, 96, PixelFormats.Bgra32, null, new byte[4], 4);
}
