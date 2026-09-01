using MusicPlayer.ViewModels;

namespace MusicPlayer.Tests;

/// <summary>
/// <see cref="MainViewModel"/> の単体テスト。フェイクの各サービスで検証する。
/// </summary>
public class MainViewModelTests
{
	private static MainViewModel CreateViewModel(
		FakeMediaPlayerController? player = null,
		FakeAudioFileScanner? scanner = null,
		FakeFolderPicker? folderPicker = null,
		Random? random = null) =>
		new(
			player ?? new FakeMediaPlayerController(),
			scanner ?? new FakeAudioFileScanner(),
			folderPicker ?? new FakeFolderPicker(),
			random ?? new Random(1));

	/// <summary>
	/// パス条件: コンストラクタ実行時、プレイリストは空であること
	/// </summary>
	[Fact]
	public void コンストラクタ_初期化時プレイリストは空()
	{
		var viewModel = CreateViewModel();

		Assert.Empty(viewModel.Playlist);
	}

	/// <summary>
	/// パス条件: LoadFolderCommand実行時、ダイアログがキャンセルされた場合プレイリストに何も追加されないこと
	/// </summary>
	[Fact]
	public async Task LoadFolderCommand_キャンセル時は何も追加されない()
	{
		var folderPicker = new FakeFolderPicker { PathToReturn = null };
		var viewModel = CreateViewModel(folderPicker: folderPicker);

		viewModel.LoadFolderCommand.Execute(null);
		await Task.Delay(20);

		Assert.Empty(viewModel.Playlist);
	}

	/// <summary>
	/// パス条件: LoadFolderCommand実行時、フォルダが選択された場合スキャンした曲がプレイリストに追加されること
	/// </summary>
	[Fact]
	public async Task LoadFolderCommand_フォルダ選択時スキャンした曲が追加される()
	{
		var folderPicker = new FakeFolderPicker { PathToReturn = @"C:\Music" };
		var scanner = new FakeAudioFileScanner { PathsToReturn = [@"C:\Music\曲A.mp3", @"C:\Music\曲B.mp3"] };
		var viewModel = CreateViewModel(scanner: scanner, folderPicker: folderPicker);

		viewModel.LoadFolderCommand.Execute(null);
		await Task.Delay(20);

		Assert.Equal(2, viewModel.Playlist.Count);
		Assert.Equal("曲A", viewModel.Playlist[0].Title);
	}

	/// <summary>
	/// パス条件: SelectTrackCommand実行で選択した曲が読み込まれ再生されること
	/// </summary>
	[Fact]
	public void SelectTrackCommand_実行すると選択した曲が読み込まれ再生される()
	{
		var player = new FakeMediaPlayerController();
		var scanner = new FakeAudioFileScanner { PathsToReturn = [@"C:\Music\曲A.mp3"] };
		var folderPicker = new FakeFolderPicker { PathToReturn = @"C:\Music" };
		var viewModel = CreateViewModel(player, scanner, folderPicker);
		viewModel.LoadFolderCommand.Execute(null);

		viewModel.SelectTrackCommand.Execute(viewModel.Playlist[0]);

		Assert.Equal(@"C:\Music\曲A.mp3", player.LastLoadedPath);
		Assert.Equal(1, player.PlayCallCount);
		Assert.Equal(viewModel.Playlist[0], viewModel.CurrentTrack);
		Assert.True(viewModel.IsPlaying);
	}

	/// <summary>
	/// パス条件: SelectTrackCommand実行(CurrentTrackの変化)で、PlayPauseCommand/StopCommandの
	/// CanExecuteChangedが発火すること。WPFの<c>Button</c>はこのイベントを契機に<c>IsEnabled</c>を
	/// 再評価するため、単に<c>CanExecute()</c>を直接呼んで確認するだけでは不十分(常に最新状態を返してしまい
	/// 「発火し忘れ」を検出できない)。実機のUI Automation検証でボタンが無効化されたままになる不具合として発見した。
	/// </summary>
	[Fact]
	public void SelectTrackCommand_実行するとPlayPauseCommandとStopCommandのCanExecuteChangedが発火する()
	{
		var scanner = new FakeAudioFileScanner { PathsToReturn = [@"C:\Music\曲A.mp3"] };
		var folderPicker = new FakeFolderPicker { PathToReturn = @"C:\Music" };
		var viewModel = CreateViewModel(scanner: scanner, folderPicker: folderPicker);
		viewModel.LoadFolderCommand.Execute(null);
		var playPauseRaised = false;
		var stopRaised = false;
		viewModel.PlayPauseCommand.CanExecuteChanged += (_, _) => playPauseRaised = true;
		viewModel.StopCommand.CanExecuteChanged += (_, _) => stopRaised = true;

		viewModel.SelectTrackCommand.Execute(viewModel.Playlist[0]);

		Assert.True(playPauseRaised);
		Assert.True(stopRaised);
	}

	/// <summary>
	/// パス条件: 再生中の曲をRemoveTrackCommandで削除した(CurrentTrackがnullに変化した)ときも、
	/// PlayPauseCommand/StopCommandのCanExecuteChangedが発火すること
	/// </summary>
	[Fact]
	public void RemoveTrackCommand_再生中の曲を削除するとCanExecuteChangedが発火する()
	{
		var scanner = new FakeAudioFileScanner { PathsToReturn = [@"C:\Music\曲A.mp3"] };
		var folderPicker = new FakeFolderPicker { PathToReturn = @"C:\Music" };
		var viewModel = CreateViewModel(scanner: scanner, folderPicker: folderPicker);
		viewModel.LoadFolderCommand.Execute(null);
		viewModel.SelectTrackCommand.Execute(viewModel.Playlist[0]);
		var playPauseRaised = false;
		viewModel.PlayPauseCommand.CanExecuteChanged += (_, _) => playPauseRaised = true;

		viewModel.RemoveTrackCommand.Execute(viewModel.CurrentTrack);

		Assert.True(playPauseRaised);
	}

	/// <summary>
	/// パス条件: PlayPauseCommand実行で再生中はPause、停止中はPlayが呼ばれること
	/// </summary>
	[Fact]
	public void PlayPauseCommand_再生中と停止中でPlayPauseが切り替わる()
	{
		var player = new FakeMediaPlayerController();
		var scanner = new FakeAudioFileScanner { PathsToReturn = [@"C:\Music\曲A.mp3"] };
		var folderPicker = new FakeFolderPicker { PathToReturn = @"C:\Music" };
		var viewModel = CreateViewModel(player, scanner, folderPicker);
		viewModel.LoadFolderCommand.Execute(null);
		viewModel.SelectTrackCommand.Execute(viewModel.Playlist[0]);

		viewModel.PlayPauseCommand.Execute(null);
		Assert.Equal(1, player.PauseCallCount);
		Assert.False(viewModel.IsPlaying);

		viewModel.PlayPauseCommand.Execute(null);
		Assert.Equal(2, player.PlayCallCount);
		Assert.True(viewModel.IsPlaying);
	}

	/// <summary>
	/// パス条件: StopCommand実行でPlayer.Stopが呼ばれIsPlayingがfalseになること
	/// </summary>
	[Fact]
	public void StopCommand_実行するとPlayerのStopが呼ばれる()
	{
		var player = new FakeMediaPlayerController();
		var scanner = new FakeAudioFileScanner { PathsToReturn = [@"C:\Music\曲A.mp3"] };
		var folderPicker = new FakeFolderPicker { PathToReturn = @"C:\Music" };
		var viewModel = CreateViewModel(player, scanner, folderPicker);
		viewModel.LoadFolderCommand.Execute(null);
		viewModel.SelectTrackCommand.Execute(viewModel.Playlist[0]);

		viewModel.StopCommand.Execute(null);

		Assert.Equal(1, player.StopCallCount);
		Assert.False(viewModel.IsPlaying);
	}

	/// <summary>
	/// パス条件: NextCommand実行で次の曲が再生されること
	/// </summary>
	[Fact]
	public void NextCommand_実行すると次の曲が再生される()
	{
		var player = new FakeMediaPlayerController();
		var scanner = new FakeAudioFileScanner { PathsToReturn = [@"C:\Music\曲A.mp3", @"C:\Music\曲B.mp3"] };
		var folderPicker = new FakeFolderPicker { PathToReturn = @"C:\Music" };
		var viewModel = CreateViewModel(player, scanner, folderPicker);
		viewModel.LoadFolderCommand.Execute(null);
		viewModel.SelectTrackCommand.Execute(viewModel.Playlist[0]);

		viewModel.NextCommand.Execute(null);

		Assert.Equal(viewModel.Playlist[1], viewModel.CurrentTrack);
	}

	/// <summary>
	/// パス条件: MediaEnded受信で自動的に次の曲が再生されること
	/// </summary>
	[Fact]
	public void MediaEnded受信で自動的に次の曲が再生される()
	{
		var player = new FakeMediaPlayerController();
		var scanner = new FakeAudioFileScanner { PathsToReturn = [@"C:\Music\曲A.mp3", @"C:\Music\曲B.mp3"] };
		var folderPicker = new FakeFolderPicker { PathToReturn = @"C:\Music" };
		var viewModel = CreateViewModel(player, scanner, folderPicker);
		viewModel.LoadFolderCommand.Execute(null);
		viewModel.SelectTrackCommand.Execute(viewModel.Playlist[0]);

		player.RaiseMediaEnded();

		Assert.Equal(viewModel.Playlist[1], viewModel.CurrentTrack);
	}

	/// <summary>
	/// パス条件: リピートOFFで最後の曲のMediaEnded後は停止する(IsPlayingがfalseになる)こと
	/// </summary>
	[Fact]
	public void MediaEnded受信でリピートOFFの最後の曲は停止する()
	{
		var player = new FakeMediaPlayerController();
		var scanner = new FakeAudioFileScanner { PathsToReturn = [@"C:\Music\曲A.mp3"] };
		var folderPicker = new FakeFolderPicker { PathToReturn = @"C:\Music" };
		var viewModel = CreateViewModel(player, scanner, folderPicker);
		viewModel.LoadFolderCommand.Execute(null);
		viewModel.SelectTrackCommand.Execute(viewModel.Playlist[0]);

		player.RaiseMediaEnded();

		Assert.False(viewModel.IsPlaying);
	}

	/// <summary>
	/// パス条件: RemoveTrackCommand実行で指定した曲がプレイリストから削除されること
	/// </summary>
	[Fact]
	public void RemoveTrackCommand_実行すると指定曲が削除される()
	{
		var scanner = new FakeAudioFileScanner { PathsToReturn = [@"C:\Music\曲A.mp3", @"C:\Music\曲B.mp3"] };
		var folderPicker = new FakeFolderPicker { PathToReturn = @"C:\Music" };
		var viewModel = CreateViewModel(scanner: scanner, folderPicker: folderPicker);
		viewModel.LoadFolderCommand.Execute(null);
		var track = viewModel.Playlist[0];

		viewModel.RemoveTrackCommand.Execute(track);

		Assert.Single(viewModel.Playlist);
		Assert.DoesNotContain(track, viewModel.Playlist);
	}

	/// <summary>
	/// パス条件: 再生中の曲をRemoveTrackCommandで削除すると停止すること
	/// </summary>
	[Fact]
	public void RemoveTrackCommand_再生中の曲を削除すると停止する()
	{
		var player = new FakeMediaPlayerController();
		var scanner = new FakeAudioFileScanner { PathsToReturn = [@"C:\Music\曲A.mp3"] };
		var folderPicker = new FakeFolderPicker { PathToReturn = @"C:\Music" };
		var viewModel = CreateViewModel(player, scanner, folderPicker);
		viewModel.LoadFolderCommand.Execute(null);
		viewModel.SelectTrackCommand.Execute(viewModel.Playlist[0]);

		viewModel.RemoveTrackCommand.Execute(viewModel.CurrentTrack);

		Assert.Equal(1, player.StopCallCount);
		Assert.Null(viewModel.CurrentTrack);
	}

	/// <summary>
	/// パス条件: MoveUpCommand/MoveDownCommandでプレイリストの並びが変わること
	/// </summary>
	[Fact]
	public void MoveUpMoveDownCommand_実行すると並びが変わる()
	{
		var scanner = new FakeAudioFileScanner { PathsToReturn = [@"C:\Music\曲A.mp3", @"C:\Music\曲B.mp3"] };
		var folderPicker = new FakeFolderPicker { PathToReturn = @"C:\Music" };
		var viewModel = CreateViewModel(scanner: scanner, folderPicker: folderPicker);
		viewModel.LoadFolderCommand.Execute(null);
		var trackB = viewModel.Playlist[1];

		viewModel.MoveUpCommand.Execute(trackB);

		Assert.Equal(trackB, viewModel.Playlist[0]);

		viewModel.MoveDownCommand.Execute(trackB);

		Assert.Equal(trackB, viewModel.Playlist[1]);
	}

	/// <summary>
	/// パス条件: ToggleRepeatCommandでOff→All→One→Offの順に切り替わること
	/// </summary>
	[Fact]
	public void ToggleRepeatCommand_OffAllOneの順に切り替わる()
	{
		var viewModel = CreateViewModel();

		Assert.Equal(Models.RepeatMode.Off, viewModel.RepeatMode);

		viewModel.ToggleRepeatCommand.Execute(null);
		Assert.Equal(Models.RepeatMode.All, viewModel.RepeatMode);

		viewModel.ToggleRepeatCommand.Execute(null);
		Assert.Equal(Models.RepeatMode.One, viewModel.RepeatMode);

		viewModel.ToggleRepeatCommand.Execute(null);
		Assert.Equal(Models.RepeatMode.Off, viewModel.RepeatMode);
	}

	/// <summary>
	/// パス条件: ToggleShuffleCommandでIsShuffleが反転すること
	/// </summary>
	[Fact]
	public void ToggleShuffleCommand_実行するとIsShuffleが反転する()
	{
		var viewModel = CreateViewModel();

		viewModel.ToggleShuffleCommand.Execute(null);

		Assert.True(viewModel.IsShuffle);
	}

	/// <summary>
	/// パス条件: Positionを変更するとPlayer.Positionに反映されること
	/// </summary>
	[Fact]
	public void Position_変更するとPlayerのPositionに反映される()
	{
		var player = new FakeMediaPlayerController();
		var viewModel = CreateViewModel(player);

		viewModel.Position = TimeSpan.FromSeconds(30);

		Assert.Equal(TimeSpan.FromSeconds(30), player.Position);
	}

	/// <summary>
	/// パス条件: ReportPositionはPositionプロパティを更新するが、Player.Positionへは書き戻さないこと
	/// </summary>
	[Fact]
	public void ReportPosition_Positionを更新するがPlayerへは書き戻さない()
	{
		var player = new FakeMediaPlayerController { Position = TimeSpan.FromSeconds(5) };
		var viewModel = CreateViewModel(player);

		viewModel.ReportPosition(TimeSpan.FromSeconds(10));

		Assert.Equal(TimeSpan.FromSeconds(10), viewModel.Position);
		Assert.Equal(TimeSpan.FromSeconds(5), player.Position);
	}

	/// <summary>
	/// パス条件: MediaOpened受信でDurationがPlayerのDurationに更新されること
	/// </summary>
	[Fact]
	public void MediaOpened受信でDurationが更新される()
	{
		var player = new FakeMediaPlayerController { Duration = TimeSpan.FromMinutes(3) };
		var viewModel = CreateViewModel(player);

		player.RaiseMediaOpened();

		Assert.Equal(TimeSpan.FromMinutes(3), viewModel.Duration);
	}
}
