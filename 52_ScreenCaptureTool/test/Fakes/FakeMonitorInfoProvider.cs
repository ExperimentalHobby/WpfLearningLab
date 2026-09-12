using ScreenCaptureTool.Models;
using ScreenCaptureTool.Services;

namespace ScreenCaptureTool.Tests.Fakes;

/// <summary>
/// テスト用の<see cref="IMonitorInfoProvider"/>フェイク実装。指定したモニタ一覧をそのまま返す。
/// </summary>
public class FakeMonitorInfoProvider : IMonitorInfoProvider
{
	public IReadOnlyList<MonitorInfo> MonitorsToReturn { get; set; } = [];

	public IReadOnlyList<MonitorInfo> GetMonitors() => MonitorsToReturn;
}
