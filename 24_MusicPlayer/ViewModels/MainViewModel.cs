using System.Collections.ObjectModel;
using MusicPlayer.Models;
using MusicPlayer.Services;

namespace MusicPlayer.ViewModels;

/// <summary>
/// ミュージックプレイヤーのメイン画面のViewModel。プレイリスト管理と再生制御を
/// <see cref="IMediaPlayerController"/>越しに行う。
/// </summary>
public class MainViewModel : ObservableObject
{
	private readonly IMediaPlayerController _player;
	private readonly IAudioFileScanner _scanner;
	private readonly IFolderPicker _folderPicker;
	private readonly Random _random;

	private int _currentIndex = -1;
	private Track? _currentTrack;
	private bool _isPlaying;
	private RepeatMode _repeatMode;
	private bool _isShuffle;
	private Track? _selectedTrack;
	private TimeSpan _position;
	private TimeSpan _duration;
	private string _errorMessage = string.Empty;

	/// <summary>
	/// ViewModelを初期化する。
	/// </summary>
	/// <param name="player">実際のメディア再生を担うコントローラー。</param>
	/// <param name="scanner">フォルダ内の音声ファイル列挙を担うスキャナー。</param>
	/// <param name="folderPicker">フォルダ選択ダイアログを担うサービス。</param>
	/// <param name="random">曲送り・シャッフルに使う乱数生成器。テストではシード固定のものを渡す。</param>
	public MainViewModel(IMediaPlayerController player, IAudioFileScanner scanner, IFolderPicker folderPicker, Random random)
	{
		_player = player;
		_scanner = scanner;
		_folderPicker = folderPicker;
		_random = random;

		_player.MediaEnded += (_, _) => Next();
		_player.MediaOpened += (_, _) => Duration = _player.Duration ?? TimeSpan.Zero;
		_player.MediaFailed += (_, ex) => OnMediaFailed(ex);

		LoadFolderCommand = new AsyncRelayCommand(LoadFolderAsync);
		PlayPauseCommand = new RelayCommand(PlayPause, () => CurrentTrack is not null);
		StopCommand = new RelayCommand(Stop, () => CurrentTrack is not null);
		NextCommand = new RelayCommand(Next, () => Playlist.Count > 0);
		PreviousCommand = new RelayCommand(Previous, () => Playlist.Count > 0);
		SelectTrackCommand = new RelayCommand<Track>(SelectTrack);
		RemoveTrackCommand = new RelayCommand<Track>(RemoveTrack);
		MoveUpCommand = new RelayCommand<Track>(MoveUp);
		MoveDownCommand = new RelayCommand<Track>(MoveDown);
		ToggleRepeatCommand = new RelayCommand(ToggleRepeat);
		ToggleShuffleCommand = new RelayCommand(() => IsShuffle = !IsShuffle);
	}

	/// <summary>プレイリスト。</summary>
	public ObservableCollection<Track> Playlist { get; } = [];

	/// <summary>現在再生中(または選択中)のトラック。</summary>
	public Track? CurrentTrack
	{
		get => _currentTrack;
		private set => SetProperty(ref _currentTrack, value);
	}

	/// <summary>再生中かどうか。</summary>
	public bool IsPlaying
	{
		get => _isPlaying;
		private set => SetProperty(ref _isPlaying, value);
	}

	/// <summary>リピートモード。</summary>
	public RepeatMode RepeatMode
	{
		get => _repeatMode;
		private set => SetProperty(ref _repeatMode, value);
	}

	/// <summary>シャッフルが有効かどうか。</summary>
	public bool IsShuffle
	{
		get => _isShuffle;
		private set => SetProperty(ref _isShuffle, value);
	}

	/// <summary>プレイリストで選択中のトラック(削除・並び替え対象の選択に使う)。</summary>
	public Track? SelectedTrack
	{
		get => _selectedTrack;
		set => SetProperty(ref _selectedTrack, value);
	}

	/// <summary>
	/// 現在の再生位置。setterはシークバー操作等、ユーザー起点の変更を想定し<see cref="IMediaPlayerController.Position"/>に反映する。
	/// 再生中の位置更新には<see cref="ReportPosition"/>を使う(無限ループ防止のため)。
	/// </summary>
	public TimeSpan Position
	{
		get => _position;
		set
		{
			if (SetProperty(ref _position, value))
			{
				_player.Position = value;
			}
		}
	}

	/// <summary>読み込み済みトラックの長さ。</summary>
	public TimeSpan Duration
	{
		get => _duration;
		private set => SetProperty(ref _duration, value);
	}

	/// <summary>直近の再生失敗などで発生したエラーメッセージ。エラーがなければ空文字列。</summary>
	public string ErrorMessage
	{
		get => _errorMessage;
		private set => SetProperty(ref _errorMessage, value);
	}

	/// <summary>フォルダを選択し、含まれる音声ファイルをプレイリストへ読み込むコマンド。</summary>
	public AsyncRelayCommand LoadFolderCommand { get; }

	/// <summary>再生/一時停止を切り替えるコマンド。</summary>
	public RelayCommand PlayPauseCommand { get; }

	/// <summary>再生を停止するコマンド。</summary>
	public RelayCommand StopCommand { get; }

	/// <summary>次の曲を再生するコマンド。</summary>
	public RelayCommand NextCommand { get; }

	/// <summary>前の曲を再生するコマンド。</summary>
	public RelayCommand PreviousCommand { get; }

	/// <summary>指定したトラックを選択して再生するコマンド。</summary>
	public RelayCommand<Track> SelectTrackCommand { get; }

	/// <summary>指定したトラックをプレイリストから削除するコマンド。</summary>
	public RelayCommand<Track> RemoveTrackCommand { get; }

	/// <summary>指定したトラックを1つ上に移動するコマンド。</summary>
	public RelayCommand<Track> MoveUpCommand { get; }

	/// <summary>指定したトラックを1つ下に移動するコマンド。</summary>
	public RelayCommand<Track> MoveDownCommand { get; }

	/// <summary>リピートモードをOff→All→One→Offの順に切り替えるコマンド。</summary>
	public RelayCommand ToggleRepeatCommand { get; }

	/// <summary>シャッフルの有効/無効を切り替えるコマンド。</summary>
	public RelayCommand ToggleShuffleCommand { get; }

	/// <summary>
	/// 実際の再生位置(<see cref="IMediaPlayerController.Position"/>)の変化をViewModelへ反映する。
	/// <see cref="Position"/>のsetterとは異なり<see cref="IMediaPlayerController.Position"/>への書き戻しは行わない。
	/// </summary>
	/// <param name="position">現在の再生位置。</param>
	public void ReportPosition(TimeSpan position) => SetProperty(ref _position, position, nameof(Position));

	private async Task LoadFolderAsync()
	{
		var folder = _folderPicker.PickFolder();
		if (folder is null)
		{
			return;
		}

		var paths = await _scanner.GetAudioFilePathsAsync(folder);

		// フォルダの読み込みは「置き換え」として扱う。クリアせずに追加すると、同じフォルダを
		// 2回読み込んだ場合に曲が重複し、_currentIndexが指す位置と実際の曲がずれてしまう。
		if (CurrentTrack is not null)
		{
			Stop();
			CurrentTrack = null;
			_currentIndex = -1;
			RaisePlaybackCommandsCanExecuteChanged();
		}

		Playlist.Clear();

		foreach (var path in paths)
		{
			Playlist.Add(new Track(path));
		}
	}

	private void OnMediaFailed(Exception? exception)
	{
		IsPlaying = false;
		ErrorMessage = $"再生に失敗しました: {exception?.Message ?? "不明なエラーです。"}";
	}

	private void PlayPause()
	{
		if (IsPlaying)
		{
			_player.Pause();
			IsPlaying = false;
		}
		else
		{
			_player.Play();
			IsPlaying = true;
		}
	}

	private void Stop()
	{
		_player.Stop();
		IsPlaying = false;
	}

	private void Next() => PlayAtIndex(PlaylistNavigator.GetNextIndex(Playlist.Count, _currentIndex, RepeatMode, IsShuffle, _random));

	private void Previous() => PlayAtIndex(PlaylistNavigator.GetPreviousIndex(Playlist.Count, _currentIndex, RepeatMode, IsShuffle, _random));

	private void SelectTrack(Track? track)
	{
		if (track is null)
		{
			return;
		}

		var index = Playlist.IndexOf(track);
		if (index < 0)
		{
			return;
		}

		PlayAtIndex(index);
	}

	private void PlayAtIndex(int? index)
	{
		if (index is null)
		{
			IsPlaying = false;
			return;
		}

		_currentIndex = index.Value;
		CurrentTrack = Playlist[_currentIndex];
		_player.Load(CurrentTrack.FilePath);
		_player.Play();
		IsPlaying = true;
		RaisePlaybackCommandsCanExecuteChanged();
	}

	private void RemoveTrack(Track? track)
	{
		if (track is null)
		{
			return;
		}

		var index = Playlist.IndexOf(track);
		if (index < 0)
		{
			return;
		}

		Playlist.RemoveAt(index);
		if (index == _currentIndex)
		{
			Stop();
			CurrentTrack = null;
			_currentIndex = -1;
			RaisePlaybackCommandsCanExecuteChanged();
		}
		else if (index < _currentIndex)
		{
			_currentIndex--;
		}
	}

	private void RaisePlaybackCommandsCanExecuteChanged()
	{
		PlayPauseCommand.RaiseCanExecuteChanged();
		StopCommand.RaiseCanExecuteChanged();
	}

	private void MoveUp(Track? track)
	{
		if (track is null)
		{
			return;
		}

		var index = Playlist.IndexOf(track);
		if (index <= 0)
		{
			return;
		}

		Playlist.Move(index, index - 1);
		AdjustCurrentIndexForMove(index, index - 1);
	}

	private void MoveDown(Track? track)
	{
		if (track is null)
		{
			return;
		}

		var index = Playlist.IndexOf(track);
		if (index < 0 || index >= Playlist.Count - 1)
		{
			return;
		}

		Playlist.Move(index, index + 1);
		AdjustCurrentIndexForMove(index, index + 1);
	}

	private void AdjustCurrentIndexForMove(int oldIndex, int newIndex)
	{
		if (_currentIndex == oldIndex)
		{
			_currentIndex = newIndex;
		}
		else if (oldIndex < _currentIndex && newIndex >= _currentIndex)
		{
			_currentIndex--;
		}
		else if (oldIndex > _currentIndex && newIndex <= _currentIndex)
		{
			_currentIndex++;
		}
	}

	private void ToggleRepeat()
	{
		RepeatMode = RepeatMode switch
		{
			RepeatMode.Off => RepeatMode.All,
			RepeatMode.All => RepeatMode.One,
			_ => RepeatMode.Off,
		};
	}
}
