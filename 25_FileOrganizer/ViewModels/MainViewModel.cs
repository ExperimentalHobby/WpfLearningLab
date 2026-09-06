using System.Collections.ObjectModel;
using System.Linq;
using FileOrganizer.Models;
using FileOrganizer.Services;

namespace FileOrganizer.ViewModels;

/// <summary>
/// フォルダ監視・振り分けルール管理・処理ログ表示を行うメイン画面のViewModel。
/// </summary>
public class MainViewModel : ObservableObject, IDisposable
{
	private readonly IFileOrganizerService _organizerService;
	private readonly IDirectoryWatcher _watcher;
	private readonly IFolderPicker _folderPicker;
	private readonly IUiDispatcher _dispatcher;

	private string _watchFolder = string.Empty;
	private bool _isWatching;
	private string _newRuleExtension = string.Empty;
	private string _newRuleDestination = string.Empty;

	/// <summary>
	/// ViewModelを初期化する。
	/// </summary>
	public MainViewModel(IFileOrganizerService organizerService, IDirectoryWatcher watcher, IFolderPicker folderPicker, IUiDispatcher dispatcher)
	{
		_organizerService = organizerService;
		_watcher = watcher;
		_folderPicker = folderPicker;
		_dispatcher = dispatcher;
		_watcher.FileCreated += OnFileCreated;

		SelectFolderCommand = new RelayCommand(SelectFolder);
		AddRuleCommand = new RelayCommand(AddRule, CanAddRule);
		RemoveRuleCommand = new RelayCommand<SortingRule>(RemoveRule);
		StartWatchingCommand = new RelayCommand(StartWatching, CanStartWatching);
		StopWatchingCommand = new RelayCommand(StopWatching, CanStopWatching);
		OrganizeExistingCommand = new AsyncRelayCommand(OrganizeExistingAsync, CanOrganizeExisting);
	}

	/// <summary>監視対象フォルダのパス。<see cref="SelectFolderCommand"/>で設定する。</summary>
	public string WatchFolder
	{
		get => _watchFolder;
		private set
		{
			if (SetProperty(ref _watchFolder, value))
			{
				StartWatchingCommand.RaiseCanExecuteChanged();
				OrganizeExistingCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>監視中かどうか。</summary>
	public bool IsWatching
	{
		get => _isWatching;
		private set
		{
			if (SetProperty(ref _isWatching, value))
			{
				StartWatchingCommand.RaiseCanExecuteChanged();
				StopWatchingCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>ルール追加フォームの拡張子入力欄。</summary>
	public string NewRuleExtension
	{
		get => _newRuleExtension;
		set
		{
			if (SetProperty(ref _newRuleExtension, value))
			{
				AddRuleCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>ルール追加フォームの移動先フォルダ名入力欄。</summary>
	public string NewRuleDestination
	{
		get => _newRuleDestination;
		set
		{
			if (SetProperty(ref _newRuleDestination, value))
			{
				AddRuleCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>登録済みの振り分けルール一覧。</summary>
	public ObservableCollection<SortingRule> Rules { get; } = [];

	/// <summary>処理ログ一覧。移動できた/エラーになった結果のみ記録される。</summary>
	public ObservableCollection<OrganizeResult> Logs { get; } = [];

	/// <summary>監視対象フォルダを選択するコマンド。</summary>
	public RelayCommand SelectFolderCommand { get; }

	/// <summary>入力欄の内容から振り分けルールを追加するコマンド。</summary>
	public RelayCommand AddRuleCommand { get; }

	/// <summary>指定したルールを削除するコマンド。</summary>
	public RelayCommand<SortingRule> RemoveRuleCommand { get; }

	/// <summary>フォルダ監視を開始するコマンド。</summary>
	public RelayCommand StartWatchingCommand { get; }

	/// <summary>フォルダ監視を停止するコマンド。</summary>
	public RelayCommand StopWatchingCommand { get; }

	/// <summary>監視フォルダ直下の既存ファイルを一括で振り分けるコマンド。</summary>
	public AsyncRelayCommand OrganizeExistingCommand { get; }

	private void SelectFolder()
	{
		var folder = _folderPicker.PickFolder();
		if (folder is not null)
		{
			WatchFolder = folder;
		}
	}

	private bool CanAddRule() => !string.IsNullOrWhiteSpace(NewRuleExtension) && !string.IsNullOrWhiteSpace(NewRuleDestination);

	private void AddRule()
	{
		var extension = NewRuleExtension.StartsWith('.') ? NewRuleExtension : $".{NewRuleExtension}";
		Rules.Add(new SortingRule(extension, NewRuleDestination));
		NewRuleExtension = string.Empty;
		NewRuleDestination = string.Empty;
	}

	private void RemoveRule(SortingRule? rule)
	{
		if (rule is not null)
		{
			Rules.Remove(rule);
		}
	}

	private bool CanStartWatching() => !IsWatching && !string.IsNullOrWhiteSpace(WatchFolder);

	private void StartWatching()
	{
		_watcher.Start(WatchFolder);
		IsWatching = true;
	}

	private bool CanStopWatching() => IsWatching;

	private void StopWatching()
	{
		_watcher.Stop();
		IsWatching = false;
	}

	private bool CanOrganizeExisting() => !string.IsNullOrWhiteSpace(WatchFolder);

	private async Task OrganizeExistingAsync()
	{
		var results = await _organizerService.OrganizeExistingFilesAsync(WatchFolder, Rules.ToList());
		foreach (var result in results.Where(ShouldLog))
		{
			Logs.Add(result);
		}
	}

	private static bool ShouldLog(OrganizeResult result) => result.Moved || result.ErrorMessage is not null;

	private async void OnFileCreated(string filePath)
	{
		// FileSystemWatcherのイベントはバックグラウンドスレッドから発火する。UIスレッド所有の
		// ObservableCollectionである Rules を直接読むと、AddRule/RemoveRuleとの競合状態になりうるため
		// _dispatcher.Invoke経由でスナップショットを取得する。また、このメソッドはバックグラウンド
		// スレッドからのイベントハンドラであり呼び出し元が存在しない(async voidの例外は捕捉されず
		// アプリ全体をクラッシュさせる)ため、ここが実質的な最終防衛ラインとして例外を広く捕捉する。
		try
		{
			var rules = _dispatcher.Invoke(() => Rules.ToList());
			var watchFolder = _dispatcher.Invoke(() => WatchFolder);
			var result = await _organizerService.OrganizeFileAsync(filePath, watchFolder, rules);
			if (ShouldLog(result))
			{
				_dispatcher.Invoke(() => Logs.Add(result));
			}
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"ファイル振り分け中に予期しないエラーが発生しました: {ex}");
		}
	}

	/// <summary>
	/// フォルダ監視の購読解除と<see cref="IDirectoryWatcher"/>の破棄を行う。
	/// </summary>
	public void Dispose()
	{
		_watcher.FileCreated -= OnFileCreated;
		_watcher.Dispose();
	}
}
