using LogStreamAggregator.Services;

namespace LogStreamAggregator.Tests;

/// <summary>
/// <see cref="DummyLogGenerator"/>のテスト。
/// </summary>
public class DummyLogGeneratorTests
{
	/// <summary>
	/// パス条件: 同一シードの乱数を与えると、決定的に同じログ行が生成されること。
	/// </summary>
	[Fact]
	public void Generate_同一シードなら決定的に同じログ行を生成する()
	{
		var generatorA = new DummyLogGenerator(new Random(42));
		var generatorB = new DummyLogGenerator(new Random(42));

		for (var i = 0; i < 10; i++)
		{
			var entryA = generatorA.Generate();
			var entryB = generatorB.Generate();
			Assert.Equal(entryA.Level, entryB.Level);
			Assert.Equal(entryA.Message, entryB.Message);
		}
	}
}
