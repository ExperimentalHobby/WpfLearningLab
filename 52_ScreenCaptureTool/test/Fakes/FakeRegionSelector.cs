using ScreenCaptureTool.Models;
using ScreenCaptureTool.Services;

namespace ScreenCaptureTool.Tests.Fakes;

/// <summary>
/// テスト用の<see cref="IRegionSelector"/>フェイク実装。あらかじめ設定した結果を返す。
/// </summary>
public class FakeRegionSelector : IRegionSelector
{
	/// <summary><see cref="SelectRegion"/>が返す値(テスト用)。</summary>
	public CaptureRegion? ResultToReturn { get; set; }

	/// <inheritdoc/>
	public CaptureRegion? SelectRegion() => ResultToReturn;
}
