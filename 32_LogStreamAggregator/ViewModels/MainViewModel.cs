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
	private readonly Func<LogEntry> _generateLogEntry;
	private CancellationTokenSource? _cancellationTokenSource;
	private LogAggregator _aggregator = new();
	private bool _isRunning;
	private int _totalCount;
	private string _errorMessage = string.Empty;

	/// <summary>
	/// <see cref="MainViewModel"/>を初期化する。
	/// </summary>
	/// <param name="dispatcher">UIスレッドへのマーシャリング処理。</param>
	/// <param name="logEntryGenerator">
	/// ログ行の生成処理。省略時は既定の<see cref="DummyLogGenerator"/>を使う。
	/// テストから例外を投げる生成処理を注入できるようにするための拡張ポイント。
	/// </param>
	public MainViewModel(IUiDispatcher dispatcher, Func<LogEntry>? logEntryGenerator = null)
	{
		_dispatcher = dispatcher;
		_generateLogEntry = logEntryGenerator ?? new DummyLogGenerator(new Random()).Generate;
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

	/// <summary>直近の操作で発生したエラーメッセージ。エラーがなければ空文字列。</summary>
	public string ErrorMessage { get => _errorMessage; private set => SetProperty(ref _errorMessage, value); }

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
		var aggregator = new LogAggregator();
		_aggregator = aggregator;
		TotalCount = 0;
		ErrorMessage = string.Empty;
		RecentLogs.Clear();
		RefreshCountViews(aggregator);

		var pipeline = new LogStreamPipeline(ChannelCapacity);
		var cts = new CancellationTokenSource();
		_cancellationTokenSource = cts;
		IsRunning = true;

		var producerTask = RunProducerAsync(pipeline, cts.Token);
		var consumerTask = RunConsumerAsync(pipeline, aggregator, cts.Token);
		_ = ObserveCompletionAsync(producerTask, consumerTask, cts);
	}

	private void Stop()
	{
		_cancellationTokenSource?.Cancel();
		IsRunning = false;
	}

	/// <summary>
	/// Producer/Consumerタスクの完了を観測し、Stop操作以外の予期しない例外をUIへ反映する。
	/// あわせて、両タスクが完了した後に<see cref="CancellationTokenSource"/>を破棄する
	/// (Stop()自体はタスク完了を待たないため、後始末をここで行う)。
	/// </summary>
	private async Task ObserveCompletionAsync(Task producerTask, Task consumerTask, CancellationTokenSource cts)
	{
		try
		{
			await Task.WhenAll(producerTask, consumerTask);
		}
		catch (OperationCanceledException)
		{
			// Stop操作による正常なキャンセル。
		}
		catch (Exception ex)
		{
			_dispatcher.Invoke(() =>
			{
				ErrorMessage = $"予期しないエラーが発生しました: {ex.Message}";
				IsRunning = false;
			});
		}
		finally
		{
			// 既に次のStart()で新しいCTSに差し替えられていた場合、現在稼働中の実行に影響しないよう
			// フィールドのnull化は行わず、破棄のみ行う。
			if (ReferenceEquals(_cancellationTokenSource, cts))
			{
				_cancellationTokenSource = null;
			}

			cts.Dispose();
		}
	}

	private async Task RunProducerAsync(LogStreamPipeline pipeline, CancellationToken token)
	{
		try
		{
			while (!token.IsCancellationRequested)
			{
				await pipeline.Writer.WriteAsync(_generateLogEntry(), token);
				await Task.Delay(ProduceInterval, token);
			}
		}
		finally
		{
			pipeline.Writer.TryComplete();
		}
	}

	private async Task RunConsumerAsync(LogStreamPipeline pipeline, LogAggregator aggregator, CancellationToken token)
	{
		await foreach (var entry in pipeline.Reader.ReadAllAsync(token))
		{
			aggregator.Add(entry);
			_dispatcher.Invoke(() => OnEntryAggregated(entry, aggregator, token));
		}
	}

	private void OnEntryAggregated(LogEntry entry, LogAggregator aggregator, CancellationToken token)
	{
		// 古い実行のConsumerからの遅延したUI更新が、既にStop済みの状態を上書きしないようにする。
		if (token.IsCancellationRequested)
		{
			return;
		}

		TotalCount = aggregator.TotalCount;
		RefreshCountViews(aggregator);

		RecentLogs.Insert(0, $"[{entry.Timestamp:HH:mm:ss}] {entry.Level}: {entry.Message}");
		while (RecentLogs.Count > MaxRecentLogs)
		{
			RecentLogs.RemoveAt(RecentLogs.Count - 1);
		}
	}

	private void RefreshCountViews(LogAggregator aggregator)
	{
		var countsByLevel = aggregator.CountsByLevel;
		var keywordCounts = aggregator.KeywordCounts;
		for (var i = 0; i < CountsByLevel.Count; i++)
		{
			var level = CountsByLevel[i].Key;
			CountsByLevel[i] = new KeyValuePair<LogLevel, int>(level, countsByLevel[level]);
		}
		for (var i = 0; i < KeywordCounts.Count; i++)
		{
			var keyword = KeywordCounts[i].Key;
			KeywordCounts[i] = new KeyValuePair<string, int>(keyword, keywordCounts[keyword]);
		}
	}
}
