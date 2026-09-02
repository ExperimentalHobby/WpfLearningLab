using NetworkMonitor.Models;

namespace NetworkMonitor.Services;

/// <summary>
/// 直近n件の<see cref="BandwidthSample"/>のみを保持するリングバッファ的な履歴。
/// 上限を超えて追加すると最古のサンプルを破棄する。
/// </summary>
public class BandwidthHistory
{
	private readonly int _maxSampleCount;
	private readonly List<BandwidthSample> _samples = [];

	/// <summary>
	/// 履歴を初期化する。
	/// </summary>
	/// <param name="maxSampleCount">保持する最大サンプル数。</param>
	public BandwidthHistory(int maxSampleCount)
	{
		if (maxSampleCount <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(maxSampleCount), "maxSampleCountは1以上である必要があります。");
		}

		_maxSampleCount = maxSampleCount;
	}

	/// <summary>保持しているサンプル一覧(古い順)。</summary>
	public IReadOnlyList<BandwidthSample> Samples => _samples;

	/// <summary>
	/// サンプルを追加する。上限を超える場合は最古のサンプルを破棄する。
	/// </summary>
	public void Add(BandwidthSample sample)
	{
		_samples.Add(sample);
		if (_samples.Count > _maxSampleCount)
		{
			_samples.RemoveAt(0);
		}
	}

	/// <summary>
	/// 保持しているサンプルを全て破棄する。
	/// </summary>
	public void Clear() => _samples.Clear();
}
