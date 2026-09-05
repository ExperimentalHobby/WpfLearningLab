namespace ScreenCaptureTool.Models;

/// <summary>
/// 物理ピクセル座標系でのキャプチャ範囲。
/// </summary>
/// <param name="Left">左端の物理X座標。</param>
/// <param name="Top">上端の物理Y座標。</param>
/// <param name="Width">幅(物理ピクセル)。</param>
/// <param name="Height">高さ(物理ピクセル)。</param>
public record CaptureRegion(int Left, int Top, int Width, int Height);
