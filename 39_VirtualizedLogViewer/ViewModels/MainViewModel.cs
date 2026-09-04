using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using VirtualizedLogViewer.Models;
using VirtualizedLogViewer.Services;

namespace VirtualizedLogViewer.ViewModels;

/// <summary>
/// 大量ログビューア(仮想化)のメインViewModel。
/// </summary>
public class MainViewModel : ObservableObject
{
	private static readonly string[] Levels = ["INFO", "WARN", "ERROR", "DEBUG"];

	private List<LogLine> _allLines = [];
	private string _lineCountInput = "100000";
	private string _keywordFilter = string.Empty;
	private string _levelFilter = LogLineFilter.AllLevels;
	private string _jumpLineInput = string.Empty;
	private bool _isVirtualizationEnabled = true;
	private string _statusText = string.Empty;
	private bool _isBusy;

	/// <summary>
	/// <see cref="MainViewModel"/>を初期化する。
	/// </summary>
	public MainViewModel()
	{
		GenerateCommand = new AsyncRelayCommand(GenerateAsync, () => !IsBusy);
		ApplyFilterCommand = new AsyncRelayCommand(ApplyFilterAsync, () => !IsBusy && _allLines.Count > 0);
		JumpToLineCommand = new RelayCommand(JumpToLine, () => DisplayedLines.Count > 0);
	}

	/// <summary>フィルタ適用後に表示中のログ行(この一覧をUI仮想化で表示する)。</summary>
	public ObservableCollection<LogLine> DisplayedLines { get; } = [];

	/// <summary>選択可能なログレベル(先頭は絞り込み無し)。</summary>
	public IReadOnlyList<string> LevelOptions { get; } = [LogLineFilter.AllLevels, .. Levels];

	/// <summary>生成する行数の入力値。</summary>
	public string LineCountInput { get => _lineCountInput; set => SetProperty(ref _lineCountInput, value); }

	/// <summary>キーワード絞り込みの入力値。</summary>
	public string KeywordFilter { get => _keywordFilter; set => SetProperty(ref _keywordFilter, value); }

	/// <summary>レベル絞り込みの選択値。</summary>
	public string LevelFilter { get => _levelFilter; set => SetProperty(ref _levelFilter, value); }

	/// <summary>ジャンプ先の行番号入力値。</summary>
	public string JumpLineInput { get => _jumpLineInput; set => SetProperty(ref _jumpLineInput, value); }

	/// <summary>UI仮想化(<c>VirtualizingPanel.IsVirtualizing</c>)を有効にするかどうか。</summary>
	public bool IsVirtualizationEnabled { get => _isVirtualizationEnabled; set => SetProperty(ref _isVirtualizationEnabled, value); }

	/// <summary>生成・絞り込みの結果や所要時間を表示するステータステキスト。</summary>
	public string StatusText { get => _statusText; private set => SetProperty(ref _statusText, value); }

	/// <summary>生成・絞り込み処理中かどうか。</summary>
	public bool IsBusy
	{
		get => _isBusy;
		private set
		{
			if (SetProperty(ref _isBusy, value))
			{
				((AsyncRelayCommand)GenerateCommand).RaiseCanExecuteChanged();
				((AsyncRelayCommand)ApplyFilterCommand).RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>ダミーログを生成するコマンド。</summary>
	public ICommand GenerateCommand { get; }

	/// <summary>キーワード・レベルで絞り込むコマンド。</summary>
	public ICommand ApplyFilterCommand { get; }

	/// <summary>指定した行番号へジャンプするコマンド。</summary>
	public ICommand JumpToLineCommand { get; }

	/// <summary>
	/// ジャンプ実行時、表示中リスト内の0始まりインデックスを通知する。Viewはこれを受けてスクロールする。
	/// </summary>
	public event Action<int>? JumpRequested;

	private async Task GenerateAsync()
	{
		if (!int.TryParse(LineCountInput, out var count) || count <= 0)
		{
			StatusText = "行数には正の整数を指定してください。";
			return;
		}

		IsBusy = true;
		try
		{
			var stopwatch = Stopwatch.StartNew();
			var lines = await Task.Run(() =>
			{
				var random = new Random();
				var generated = new List<LogLine>(count);
				for (var i = 1; i <= count; i++)
				{
					generated.Add(new LogLine(i, Levels[random.Next(Levels.Length)], $"Sample log message number {i}"));
				}
				return generated;
			});
			stopwatch.Stop();

			_allLines = lines;
			await ApplyFilterCoreAsync();
			StatusText = $"{count:N0}件を{stopwatch.ElapsedMilliseconds}msで生成しました。";
		}
		finally
		{
			IsBusy = false;
			((AsyncRelayCommand)ApplyFilterCommand).RaiseCanExecuteChanged();
		}
	}

	private async Task ApplyFilterAsync()
	{
		IsBusy = true;
		try
		{
			var stopwatch = Stopwatch.StartNew();
			await ApplyFilterCoreAsync();
			stopwatch.Stop();
			StatusText = $"{DisplayedLines.Count:N0} / {_allLines.Count:N0}件を{stopwatch.ElapsedMilliseconds}msで絞り込みました。";
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task ApplyFilterCoreAsync()
	{
		var keyword = KeywordFilter;
		var level = LevelFilter;
		var allLines = _allLines;
		var filtered = await Task.Run(() => LogLineFilter.Filter(allLines, keyword, level));

		DisplayedLines.Clear();
		foreach (var line in filtered)
		{
			DisplayedLines.Add(line);
		}
		((RelayCommand)JumpToLineCommand).RaiseCanExecuteChanged();
	}

	private void JumpToLine()
	{
		if (!LineJumpCalculator.TryParseLineNumber(JumpLineInput, out var lineNumber))
		{
			StatusText = "有効な行番号を指定してください。";
			return;
		}

		var index = LineJumpCalculator.FindDisplayIndex(DisplayedLines, lineNumber);
		if (index < 0)
		{
			StatusText = $"{lineNumber}行目は現在の表示対象にありません(フィルタで除外されている可能性があります)。";
			return;
		}

		JumpRequested?.Invoke(index);
	}
}
