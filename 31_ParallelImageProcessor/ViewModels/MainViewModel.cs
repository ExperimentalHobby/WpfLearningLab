using System.IO;
using System.Windows.Input;
using ParallelImageProcessor.Models;
using ParallelImageProcessor.Services;

namespace ParallelImageProcessor.ViewModels;

/// <summary>
/// 並列画像バッチ処理ツールのメインViewModel。
/// </summary>
public class MainViewModel : ObservableObject
{
	private readonly IImageBatchProcessor _processor;
	private readonly IFolderPicker _folderPicker;
	private CancellationTokenSource? _cancellationTokenSource;

	private static readonly string[] SupportedExtensions = [".png", ".jpg", ".jpeg", ".bmp"];

	private string? _sourceFolder;
	private string? _destinationFolder;
	private bool _resizeEnabled = true;
	private int _targetWidth = 800;
	private int _targetHeight = 600;
	private bool _grayscaleEnabled;
	private bool _isProcessing;
	private double _progressPercentage;
	private string _progressText = string.Empty;
	private string _resultSummaryText = string.Empty;
	private int _lastReportedCompleted;

	/// <summary>
	/// <see cref="MainViewModel"/>を初期化する。
	/// </summary>
	public MainViewModel(IImageBatchProcessor processor, IFolderPicker folderPicker)
	{
		_processor = processor;
		_folderPicker = folderPicker;

		SelectSourceFolderCommand = new RelayCommand(SelectSourceFolder);
		SelectDestinationFolderCommand = new RelayCommand(SelectDestinationFolder);
		StartCommand = new AsyncRelayCommand(StartAsync, CanStart);
		CancelCommand = new RelayCommand(Cancel, CanCancel);
	}

	/// <summary>処理対象フォルダ。</summary>
	public string? SourceFolder
	{
		get => _sourceFolder;
		private set
		{
			if (SetProperty(ref _sourceFolder, value))
			{
				((AsyncRelayCommand)StartCommand).RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>処理結果の保存先フォルダ。</summary>
	public string? DestinationFolder
	{
		get => _destinationFolder;
		private set
		{
			if (SetProperty(ref _destinationFolder, value))
			{
				((AsyncRelayCommand)StartCommand).RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>リサイズを行うかどうか。</summary>
	public bool ResizeEnabled { get => _resizeEnabled; set => SetProperty(ref _resizeEnabled, value); }

	/// <summary>リサイズ後の幅(px)。</summary>
	public int TargetWidth { get => _targetWidth; set => SetProperty(ref _targetWidth, value); }

	/// <summary>リサイズ後の高さ(px)。</summary>
	public int TargetHeight { get => _targetHeight; set => SetProperty(ref _targetHeight, value); }

	/// <summary>グレースケール化を行うかどうか。</summary>
	public bool GrayscaleEnabled { get => _grayscaleEnabled; set => SetProperty(ref _grayscaleEnabled, value); }

	/// <summary>処理中かどうか。</summary>
	public bool IsProcessing
	{
		get => _isProcessing;
		private set
		{
			if (SetProperty(ref _isProcessing, value))
			{
				((AsyncRelayCommand)StartCommand).RaiseCanExecuteChanged();
				((RelayCommand)CancelCommand).RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>進捗率(0〜100)。</summary>
	public double ProgressPercentage { get => _progressPercentage; private set => SetProperty(ref _progressPercentage, value); }

	/// <summary>「3 / 10」のような進捗テキスト。</summary>
	public string ProgressText { get => _progressText; private set => SetProperty(ref _progressText, value); }

	/// <summary>処理完了後のサマリテキスト。</summary>
	public string ResultSummaryText { get => _resultSummaryText; private set => SetProperty(ref _resultSummaryText, value); }

	/// <summary>処理対象フォルダを選択するコマンド。</summary>
	public ICommand SelectSourceFolderCommand { get; }

	/// <summary>保存先フォルダを選択するコマンド。</summary>
	public ICommand SelectDestinationFolderCommand { get; }

	/// <summary>バッチ処理を開始するコマンド。</summary>
	public ICommand StartCommand { get; }

	/// <summary>実行中のバッチ処理を中断するコマンド。</summary>
	public ICommand CancelCommand { get; }

	private void SelectSourceFolder()
	{
		var folder = _folderPicker.PickFolder();
		if (folder is not null)
		{
			SourceFolder = folder;
		}
	}

	private void SelectDestinationFolder()
	{
		var folder = _folderPicker.PickFolder();
		if (folder is not null)
		{
			DestinationFolder = folder;
		}
	}

	private bool CanStart() => !IsProcessing && !string.IsNullOrEmpty(SourceFolder) && !string.IsNullOrEmpty(DestinationFolder);

	private bool CanCancel() => IsProcessing;

	private async Task StartAsync()
	{
		if (SourceFolder is null || DestinationFolder is null)
		{
			return;
		}

		List<string> sourceFiles;
		try
		{
			sourceFiles = Directory.EnumerateFiles(SourceFolder)
				.Where(path => SupportedExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase))
				.ToList();
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
		{
			// AsyncRelayCommand.Executeはasync voidのため、ここで捕捉し損ねるとアプリ全体が
			// クラッシュする。
			ResultSummaryText = $"画像ファイルの一覧取得に失敗しました: {ex.Message}";
			return;
		}

		if (sourceFiles.Count == 0)
		{
			ResultSummaryText = "対象フォルダに画像ファイルが見つかりませんでした。";
			return;
		}

		var options = new ImageProcessingOptions(ResizeEnabled, TargetWidth, TargetHeight, GrayscaleEnabled);
		var progress = new Progress<BatchProgress>(OnProgressReported);

		_cancellationTokenSource = new CancellationTokenSource();
		IsProcessing = true;
		ResultSummaryText = string.Empty;
		ProgressPercentage = 0;
		ProgressText = $"0 / {sourceFiles.Count}";
		_lastReportedCompleted = 0;

		try
		{
			var result = await _processor.ProcessBatchAsync(sourceFiles, DestinationFolder, options, progress, _cancellationTokenSource.Token);
			ResultSummaryText = $"完了: 成功 {result.SuccessCount}件 / 失敗 {result.FailureCount}件 (所要時間 {result.Elapsed.TotalSeconds:F1}秒)";
		}
		catch (OperationCanceledException)
		{
			ResultSummaryText = "処理を中断しました。";
		}
		finally
		{
			IsProcessing = false;
			_cancellationTokenSource.Dispose();
			_cancellationTokenSource = null;
		}
	}

	private void OnProgressReported(BatchProgress progress)
	{
		// 並列処理のため、各ワーカースレッドからのReport呼び出しがどの順で処理されるかは保証されない。
		// 完了件数が既に報告済みの値を下回る(=古い報告が後から届いた)場合は無視し、進捗表示が
		// 一時的に後退して見えるのを防ぐ。
		if (progress.Completed < _lastReportedCompleted)
		{
			return;
		}

		_lastReportedCompleted = progress.Completed;
		ProgressPercentage = progress.Total == 0 ? 0 : progress.Completed * 100.0 / progress.Total;
		ProgressText = $"{progress.Completed} / {progress.Total}";
	}

	private void Cancel() => _cancellationTokenSource?.Cancel();
}
