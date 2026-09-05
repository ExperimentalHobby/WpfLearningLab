using ScreenCaptureTool.Models;
using ScreenCaptureTool.Services;

namespace ScreenCaptureTool.Tests.Fakes;

/// <summary>
/// テスト用の<see cref="IRegionSelector"/>フェイク実装。あらかじめ設定した結果を返す。
/// </summary>
public class FakeRegionSelector : IRegionSelector
{
	public CaptureRegion? ResultToReturn { get; set; }

	public CaptureRegion? SelectRegion() => ResultToReturn;
}
