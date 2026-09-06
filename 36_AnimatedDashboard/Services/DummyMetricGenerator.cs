using AnimatedDashboard.Models;

namespace AnimatedDashboard.Services;

/// <summary>
/// ダミーのKPI指標を生成する疑似データソース。
/// </summary>
public class DummyMetricGenerator : IMetricGenerator
{
	private readonly Random _random;

	/// <summary>
	/// <see cref="DummyMetricGenerator"/>を初期化する。
	/// </summary>
	/// <param name="random">乱数生成器。テスト時は決定的な結果を得るため固定シードを渡す。</param>
	public DummyMetricGenerator(Random random)
	{
		_random = random;
	}

	/// <inheritdoc/>
	public IReadOnlyList<KpiMetric> Generate() =>
	[
		new("売上", "万円", Math.Round(_random.NextDouble() * 1000, 1)),
		new("ユーザー数", "人", _random.Next(100, 5000)),
		new("コンバージョン率", "%", Math.Round(_random.NextDouble() * 10, 2)),
		new("エラー件数", "件", _random.Next(0, 50)),
	];
}
