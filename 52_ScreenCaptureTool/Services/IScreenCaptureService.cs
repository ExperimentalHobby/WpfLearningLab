using System.Windows.Media.Imaging;
using ScreenCaptureTool.Models;

namespace ScreenCaptureTool.Services;

/// <summary>
/// スクリーンショットの撮影を担う抽象。
/// </summary>
public interface IScreenCaptureService
{
	/// <summary>
	/// 仮想デスクトップ全体(全モニタ)をキャプチャする。
	/// </summary>
	BitmapSource CaptureFullScreen();

	/// <summary>
	/// 指定した物理ピクセル範囲をキャプチャする。
	/// </summary>
	/// <param name="region">キャプチャする物理ピクセル範囲。</param>
	BitmapSource CaptureRegion(CaptureRegion region);
}
