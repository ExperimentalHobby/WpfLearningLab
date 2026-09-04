using System.Threading.Channels;
using LogStreamAggregator.Models;

namespace LogStreamAggregator.Services;

/// <summary>
/// <see cref="System.Threading.Channels.Channel{T}"/>を使ったProducer-Consumerパイプライン。
/// 容量を制限した<see cref="BoundedChannelOptions"/>により、Consumerの処理が追いつかない場合は
/// Producer側の書き込み(<see cref="ChannelWriter{T}.WriteAsync"/>)が待機する(バックプレッシャー)。
/// </summary>
public class LogStreamPipeline
{
	private readonly Channel<LogEntry> _channel;

	/// <summary>
	/// <see cref="LogStreamPipeline"/>を初期化する。
	/// </summary>
	/// <param name="capacity">チャネルの容量。これを超える書き込みはConsumerが読み取るまで待機する。</param>
	public LogStreamPipeline(int capacity)
	{
		_channel = Channel.CreateBounded<LogEntry>(new BoundedChannelOptions(capacity)
		{
			FullMode = BoundedChannelFullMode.Wait,
		});
	}

	/// <summary>ログの書き込み口。</summary>
	public ChannelWriter<LogEntry> Writer => _channel.Writer;

	/// <summary>ログの読み取り口。</summary>
	public ChannelReader<LogEntry> Reader => _channel.Reader;

	/// <summary>
	/// 指定したログ行をすべて書き込み、完了後にチャネルを完了状態にする。
	/// </summary>
	public async Task ProduceAsync(IEnumerable<LogEntry> entries, CancellationToken cancellationToken)
	{
		foreach (var entry in entries)
		{
			await Writer.WriteAsync(entry, cancellationToken);
		}
		Writer.TryComplete();
	}

	/// <summary>
	/// チャネルが完了するまで非同期ストリームとして読み取り続け、<paramref name="aggregator"/>に集計する。
	/// </summary>
	public async Task ConsumeAsync(LogAggregator aggregator, CancellationToken cancellationToken)
	{
		await foreach (var entry in Reader.ReadAllAsync(cancellationToken))
		{
			aggregator.Add(entry);
		}
	}
}
