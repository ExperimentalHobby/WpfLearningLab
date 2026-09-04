using System.Collections.ObjectModel;
using System.Windows.Input;
using LogStreamAggregator.Models;
using LogStreamAggregator.Services;

namespace LogStreamAggregator.ViewModels;

/// <summary>
/// ログストリーム集計ツールのメインViewModel。
/// Start実行時にProducerタスク(ダミーログ生成)とConsumerタスク(集計)をバックグラウンドで起動する。
/// </summary>
public class MainViewModel : ObservableObject
{
	private const int ChannelCapacity = 20;
	private static readonly TimeSpan ProduceInterval = TimeSpan.FromMilliseconds(150);
	private const int MaxRecentLogs = 30;

	private readonly IUiDispatcher _dispatcher;
	private CancellationTokenSource? _cancellationTokenSource;
	private LogAggregator _aggregator = new();
	private bool _isRunning;
	private int _totalCount;

	/// <summary>
	/// <see cref="MainViewModel"/>を初期化する。
	/// </summary>
	public MainViewModel(IUiDispatcher dispatcher)
	{
		_dispatcher = dispatcher;
		StartCommand = new RelayCommand(Start, () => !IsRunning);
		StopCommand = new RelayCommand(Stop, () => IsRunning);

		foreach (var level in Enum.GetValues<LogLevel>())
		{
			CountsByLevel.Add(new KeyValuePair<LogLevel, int>(level, 0));
		}
		foreach (var keyword in LogAggregator.WatchedKeywords)
		{
			KeywordCounts.Add(new KeyValuePair<string, int>(keyword, 0));
		}
	}

	/// <summary>Producer/Consumerが稼働中かどうか。</summary>
	public bool IsRunning
	{
		get => _isRunning;
		private set
		{
			if (SetProperty(ref _isRunning, value))
			{
				((RelayCommand)StartCommand).RaiseCanExecuteChanged();
				((RelayCommand)StopCommand).RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>集計済みの総件数。</summary>
	public int TotalCount { get => _totalCount; private set => SetProperty(ref _totalCount, value); }

	/// <summary>ログレベル別件数(UI表示用)。</summary>
	public ObservableCollection<KeyValuePair<LogLevel, int>> CountsByLevel { get; } = [];

	/// <summary>キーワード別出現回数(UI表示用)。</summary>
	public ObservableCollection<KeyValuePair<string, int>> KeywordCounts { get; } = [];

	/// <summary>直近のログ行(表示用に件数を制限)。</summary>
	public ObservableCollection<string> RecentLogs { get; } = [];

	/// <summary>Producer/Consumerを開始するコマンド。</summary>
	public ICommand StartCommand { get; }

	/// <summary>Producer/Consumerを停止するコマンド。</summary>
	public ICommand StopCommand { get; }

	private void Start()
	{
		_aggregator = new LogAggregator();
		TotalCount = 0;
		RecentLogs.Clear();
		RefreshCountViews();

		var pipeline = new LogStreamPipeline(ChannelCapacity);
		_cancellationTokenSource = new CancellationTokenSource();
		var token = _cancellationTokenSource.Token;
		IsRunning = true;

		_ = RunProducerAsync(pipeline, token);
		_ = RunConsumerAsync(pipeline, token);
	}

	private void Stop()
	{
		_cancellationTokenSource?.Cancel();
		IsRunning = false;
	}

	private static async Task RunProducerAsync(LogStreamPipeline pipeline, CancellationToken token)
	{
		var generator = new DummyLogGenerator(new Random());
		try
		{
			while (!token.IsCancellationRequested)
			{
				await pipeline.Writer.WriteAsync(generator.Generate(), token);
				await Task.Delay(ProduceInterval, token);
			}
		}
		catch (OperationCanceledException)
		{
			// Stop操作による正常なキャンセル。
		}
		finally
		{
			pipeline.Writer.TryComplete();
		}
	}

	private async Task RunConsumerAsync(LogStreamPipeline pipeline, CancellationToken token)
	{
		try
		{
			await foreach (var entry in pipeline.Reader.ReadAllAsync(token))
			{
				_aggregator.Add(entry);
				_dispatcher.Invoke(() => OnEntryAggregated(entry));
			}
		}
		catch (OperationCanceledException)
		{
			// Stop操作による正常なキャンセル。
		}
	}

	private void OnEntryAggregated(LogEntry entry)
	{
		TotalCount = _aggregator.TotalCount;
		RefreshCountViews();

		RecentLogs.Insert(0, $"[{entry.Timestamp:HH:mm:ss}] {entry.Level}: {entry.Message}");
		while (RecentLogs.Count > MaxRecentLogs)
		{
			RecentLogs.RemoveAt(RecentLogs.Count - 1);
		}
	}

	private void RefreshCountViews()
	{
		for (var i = 0; i < CountsByLevel.Count; i++)
		{
			var level = CountsByLevel[i].Key;
			CountsByLevel[i] = new KeyValuePair<LogLevel, int>(level, _aggregator.CountsByLevel[level]);
		}
		for (var i = 0; i < KeywordCounts.Count; i++)
		{
			var keyword = KeywordCounts[i].Key;
			KeywordCounts[i] = new KeyValuePair<string, int>(keyword, _aggregator.KeywordCounts[keyword]);
		}
	}
}
