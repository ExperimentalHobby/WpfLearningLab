using NetworkMonitor.Models;
using NetworkMonitor.Services;

namespace NetworkMonitor.Tests;

/// <summary>
/// <see cref="BandwidthHistory"/> の単体テスト。
/// </summary>
public class BandwidthHistoryTests
{
	private static BandwidthSample CreateSample(int sentBytesPerSec) =>
		new(DateTime.Now, sentBytesPerSec, sentBytesPerSec);

	/// <summary>
	/// パス条件: サンプルを追加すると、Samplesに反映されること
	/// </summary>
	[Fact]
	public void Add_サンプルを追加するとSamplesに反映される()
	{
		var history = new BandwidthHistory(maxSampleCount: 3);

		history.Add(CreateSample(100));

		Assert.Single(history.Samples);
	}

	/// <summary>
	/// パス条件: 上限ちょうどの件数までは、追加したサンプルが全て保持されること
	/// </summary>
	[Fact]
	public void Add_上限ちょうどの場合すべて保持される()
	{
		var history = new BandwidthHistory(maxSampleCount: 3);

		history.Add(CreateSample(1));
		history.Add(CreateSample(2));
		history.Add(CreateSample(3));

		Assert.Equal(3, history.Samples.Count);
		Assert.Equal([1, 2, 3], history.Samples.Select(s => s.SentBytesPerSec));
	}

	/// <summary>
	/// パス条件: 上限を超えてサンプルを追加すると、最古のサンプルが破棄されること
	/// </summary>
	[Fact]
	public void Add_上限を超えると最古のサンプルが破棄される()
	{
		var history = new BandwidthHistory(maxSampleCount: 3);
		history.Add(CreateSample(1));
		history.Add(CreateSample(2));
		history.Add(CreateSample(3));

		history.Add(CreateSample(4));

		Assert.Equal(3, history.Samples.Count);
		Assert.Equal([2, 3, 4], history.Samples.Select(s => s.SentBytesPerSec));
	}

	/// <summary>
	/// パス条件: Clearを呼ぶとSamplesが空になること
	/// </summary>
	[Fact]
	public void Clear_呼び出すとSamplesが空になる()
	{
		var history = new BandwidthHistory(maxSampleCount: 3);
		history.Add(CreateSample(1));

		history.Clear();

		Assert.Empty(history.Samples);
	}
}
