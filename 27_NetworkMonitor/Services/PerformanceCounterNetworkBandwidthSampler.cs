using System.Diagnostics;

namespace NetworkMonitor.Services;

/// <summary>
/// 実の<see cref="PerformanceCounter"/>(Network Interfaceカテゴリ)を使う<see cref="INetworkBandwidthSampler"/>実装。
/// </summary>
public class PerformanceCounterNetworkBandwidthSampler : INetworkBandwidthSampler, IDisposable
{
	private const string CategoryName = "Network Interface";

	private readonly Dictionary<string, (PerformanceCounter Sent, PerformanceCounter Received)> _counters = [];

	/// <inheritdoc/>
	public IReadOnlyList<string> GetInstanceNames() => new PerformanceCounterCategory(CategoryName).GetInstanceNames();

	/// <inheritdoc/>
	public (double SentBytesPerSec, double ReceivedBytesPerSec) Sample(string instanceName)
	{
		var (sent, received) = GetOrCreateCounters(instanceName);
		return (sent.NextValue(), received.NextValue());
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		foreach (var (sent, received) in _counters.Values)
		{
			sent.Dispose();
			received.Dispose();
		}

		_counters.Clear();
	}

	private (PerformanceCounter Sent, PerformanceCounter Received) GetOrCreateCounters(string instanceName)
	{
		if (!_counters.TryGetValue(instanceName, out var counters))
		{
			counters = (
				new PerformanceCounter(CategoryName, "Bytes Sent/sec", instanceName),
				new PerformanceCounter(CategoryName, "Bytes Received/sec", instanceName));
			_counters[instanceName] = counters;
		}

		return counters;
	}
}
