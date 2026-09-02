using NetworkMonitor.Services;

namespace NetworkMonitor.Tests;

/// <summary>
/// <see cref="PerformanceCounterNetworkBandwidthSampler"/> の単体テスト。
/// 実の<see cref="System.Diagnostics.PerformanceCounter"/>(Network Interfaceカテゴリ)に対して検証する。
/// </summary>
public class PerformanceCounterNetworkBandwidthSamplerTests
{
	/// <summary>
	/// パス条件: GetInstanceNamesが例外を投げず、1件以上のインターフェース名を返すこと
	/// </summary>
	[Fact]
	public void GetInstanceNames_例外を投げず1件以上返す()
	{
		using var sampler = new PerformanceCounterNetworkBandwidthSampler();

		var instanceNames = sampler.GetInstanceNames();

		Assert.NotEmpty(instanceNames);
	}

	/// <summary>
	/// パス条件: Sampleが例外を投げず、0以上の値を返すこと
	/// </summary>
	[Fact]
	public void Sample_例外を投げず0以上の値を返す()
	{
		using var sampler = new PerformanceCounterNetworkBandwidthSampler();
		var instanceName = sampler.GetInstanceNames()[0];

		var (sent, received) = sampler.Sample(instanceName);

		Assert.True(sent >= 0);
		Assert.True(received >= 0);
	}

	/// <summary>
	/// パス条件: 同一インターフェースへの複数回のSample呼び出しでも例外を投げないこと(内部カウンターの使い回し確認)
	/// </summary>
	[Fact]
	public void Sample_同一インターフェースへの複数回呼び出しでも例外を投げない()
	{
		using var sampler = new PerformanceCounterNetworkBandwidthSampler();
		var instanceName = sampler.GetInstanceNames()[0];

		var exception = Record.Exception(() =>
		{
			sampler.Sample(instanceName);
			sampler.Sample(instanceName);
			sampler.Sample(instanceName);
		});

		Assert.Null(exception);
	}
}
