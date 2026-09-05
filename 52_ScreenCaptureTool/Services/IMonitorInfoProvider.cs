using ScreenCaptureTool.Models;

namespace ScreenCaptureTool.Services;

/// <summary>
/// 接続中のモニタの座標変換情報一覧を取得する抽象。
/// </summary>
public interface IMonitorInfoProvider
{
	/// <summary>
	/// 現在接続されている全モニタの座標変換情報を取得する。
	/// </summary>
	IReadOnlyList<MonitorInfo> GetMonitors();
}
