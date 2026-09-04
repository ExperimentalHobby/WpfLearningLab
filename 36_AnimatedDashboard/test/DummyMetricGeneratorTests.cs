using AnimatedDashboard.Services;

namespace AnimatedDashboard.Tests;

/// <summary>
/// <see cref="DummyMetricGenerator"/>のテスト。
/// </summary>
public class DummyMetricGeneratorTests
{
	/// <summary>
	/// パス条件: 同一シードの乱数を与えると、決定的に同じKPI指標一覧を生成すること。
	/// </summary>
	[Fact]
	public void Generate_同一シードなら決定的に同じ指標一覧を生成する()
	{
		var generatorA = new DummyMetricGenerator(new Random(123));
		var generatorB = new DummyMetricGenerator(new Random(123));

		var resultA = generatorA.Generate();
		var resultB = generatorB.Generate();

		Assert.Equal(resultA, resultB);
	}

	/// <summary>
	/// パス条件: 4件のKPI指標を生成すること。
	/// </summary>
	[Fact]
	public void Generate_4件の指標を生成する()
	{
		var generator = new DummyMetricGenerator(new Random());

		var result = generator.Generate();

		Assert.Equal(4, result.Count);
	}
}
