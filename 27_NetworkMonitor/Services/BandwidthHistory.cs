using NetworkMonitor.Models;

namespace NetworkMonitor.Services;

/// <summary>
/// 直近n件の<see cref="BandwidthSample"/>のみを保持するリングバッファ的な履歴。
/// 上限を超えて追加すると最古のサンプルを破棄する。
/// </summary>
public class BandwidthHistory
{
	private readonly int _maxSampleCount;
	private readonly Queue<BandwidthSample> _samples = new();

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
	public IReadOnlyCollection<BandwidthSample> Samples => _samples;

	/// <summary>
	/// サンプルを追加する。上限を超える場合は最古のサンプルを破棄する。
	/// </summary>
	public void Add(BandwidthSample sample)
	{
		// 保持件数は最大でも数十件程度だが、List.RemoveAt(0)はO(n)の要素シフトが発生するため、
		// 先頭の削除・末尾への追加がO(1)のQueueを使う。
		_samples.Enqueue(sample);
		if (_samples.Count > _maxSampleCount)
		{
			_samples.Dequeue();
		}
	}

	/// <summary>
	/// 保持しているサンプルを全て破棄する。
	/// </summary>
	public void Clear() => _samples.Clear();
}
