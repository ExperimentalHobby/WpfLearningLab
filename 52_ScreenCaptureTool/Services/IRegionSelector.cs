using ScreenCaptureTool.Models;

namespace ScreenCaptureTool.Services;

/// <summary>
/// マウスドラッグによる範囲選択UIを担う抽象。
/// </summary>
public interface IRegionSelector
{
	/// <summary>
	/// 範囲選択UIを表示し、選択された物理ピクセル範囲を返す。
	/// ユーザーが選択をキャンセルした場合は<see langword="null"/>を返す。
	/// </summary>
	CaptureRegion? SelectRegion();
}
