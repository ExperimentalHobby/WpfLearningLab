using AnimatedDashboard.Models;
using AnimatedDashboard.Services;

namespace AnimatedDashboard.Tests;

/// <summary>
/// <see cref="ViewModels.MainViewModel"/>のテスト用に、あらかじめ設定した指標一覧を順に返す
/// <see cref="IMetricGenerator"/>のフェイク。
/// </summary>
internal class FakeMetricGenerator : IMetricGenerator
{
	private readonly Queue<IReadOnlyList<KpiMetric>> _results;

	/// <summary>
	/// <see cref="FakeMetricGenerator"/>を初期化する。
	/// </summary>
	/// <param name="results">
	/// <see cref="Generate"/>が呼ばれるたびに順番に返す結果。呼び出し回数がこれを超えると最後の要素を返し続ける。
	/// </param>
	public FakeMetricGenerator(params IReadOnlyList<KpiMetric>[] results)
	{
		_results = new Queue<IReadOnlyList<KpiMetric>>(results);
	}

	/// <inheritdoc/>
	public IReadOnlyList<KpiMetric> Generate() => _results.Count > 1 ? _results.Dequeue() : _results.Peek();
}
