using LogStreamAggregator.Models;
using LogStreamAggregator.Services;

namespace LogStreamAggregator.Tests;

/// <summary>
/// <see cref="LogStreamPipeline"/>のテスト。モックを使わず、実の<see cref="System.Threading.Channels.Channel{T}"/>で
/// Producer-Consumerパイプラインの挙動(全件到達・バックプレッシャー)を検証する。
/// </summary>
public class LogStreamPipelineTests
{
	private static LogEntry MakeEntry(int index) => new(DateTime.Now, LogLevel.Info, $"msg{index}");

	/// <summary>
	/// パス条件: Produce後にConsumeすると、書き込んだ全件がAggregatorに届くこと。
	/// </summary>
	[Fact]
	public async Task ProduceAndConsume_書き込んだ全件がConsumer側に届く()
	{
		var pipeline = new LogStreamPipeline(capacity: 10);
		var entries = Enumerable.Range(0, 5).Select(MakeEntry).ToList();
		var aggregator = new LogAggregator();

		var produceTask = pipeline.ProduceAsync(entries, CancellationToken.None);
		var consumeTask = pipeline.ConsumeAsync(aggregator, CancellationToken.None);
		await Task.WhenAll(produceTask, consumeTask);

		Assert.Equal(5, aggregator.TotalCount);
	}

	/// <summary>
	/// パス条件: チャネル容量を超える書き込みは、Consumerが読み取って空きができるまで完了しない(バックプレッシャー)こと。
	/// </summary>
	[Fact]
	public async Task ProduceAsync_容量超過時はConsumerが読み取るまで完了しない()
	{
		var pipeline = new LogStreamPipeline(capacity: 1);
		var entries = Enumerable.Range(0, 3).Select(MakeEntry).ToList();

		var produceTask = pipeline.ProduceAsync(entries, CancellationToken.None);
		await Task.Delay(200);

		// 容量1のチャネルに3件書き込もうとしているため、Consumerが読み取るまでProducerは待機し続けるはず。
		Assert.False(produceTask.IsCompleted);

		var aggregator = new LogAggregator();
		await pipeline.ConsumeAsync(aggregator, CancellationToken.None);
		await produceTask;

		Assert.Equal(3, aggregator.TotalCount);
	}
}
