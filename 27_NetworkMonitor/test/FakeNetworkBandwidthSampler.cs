using NetworkMonitor.Services;

namespace NetworkMonitor.Tests;

/// <summary>
/// <see cref="ViewModels.MainViewModel"/> のテスト用に、実の<see cref="System.Diagnostics.PerformanceCounter"/>を
/// 使わない<see cref="INetworkBandwidthSampler"/>実装。
/// </summary>
public class FakeNetworkBandwidthSampler : INetworkBandwidthSampler
{
	/// <summary><see cref="GetInstanceNames"/>が返すインターフェース名一覧。</summary>
	public IReadOnlyList<string> InstanceNames { get; set; } = ["eth0", "wlan0"];

	/// <summary><see cref="Sample"/>が返す値を、呼び出し順に取り出すキュー。既定値は(100, 200)固定。</summary>
	public Queue<(double SentBytesPerSec, double ReceivedBytesPerSec)>? SampleQueue { get; set; }

	/// <summary>Sampleが呼ばれた回数。</summary>
	public int SampleCallCount { get; private set; }

	/// <summary>設定すると<see cref="GetInstanceNames"/>呼び出し時にこの例外をスローする(テスト用)。</summary>
	public Exception? GetInstanceNamesExceptionToThrow { get; set; }

	/// <summary>設定すると<see cref="Sample"/>呼び出し時にこの例外をスローする(テスト用)。</summary>
	public Exception? SampleExceptionToThrow { get; set; }

	/// <inheritdoc/>
	public IReadOnlyList<string> GetInstanceNames() =>
		GetInstanceNamesExceptionToThrow is not null ? throw GetInstanceNamesExceptionToThrow : InstanceNames;

	/// <inheritdoc/>
	public (double SentBytesPerSec, double ReceivedBytesPerSec) Sample(string instanceName)
	{
		SampleCallCount++;
		if (SampleExceptionToThrow is not null)
		{
			throw SampleExceptionToThrow;
		}

		return SampleQueue is { Count: > 0 } queue ? queue.Dequeue() : (100, 200);
	}
}
