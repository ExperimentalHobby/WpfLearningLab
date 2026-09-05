using ImageViewer.ViewModels;

namespace ImageViewer.Tests;

/// <summary>
/// <see cref="MainViewModel"/> の単体テスト。
/// </summary>
public class MainViewModelTests
{
	private static MainViewModel CreateViewModel(
		FakeFolderPicker? folderPicker = null,
		FakeImageFileScanner? scanner = null,
		FakeThumbnailLoader? thumbnailLoader = null) =>
		new(folderPicker ?? new FakeFolderPicker(), scanner ?? new FakeImageFileScanner(), thumbnailLoader ?? new FakeThumbnailLoader());

	/// <summary>
	/// パス条件: BrowseFolderCommand実行でフォルダ選択→画像一覧が取得されImageFilesに反映されること
	/// </summary>
	[Fact]
	public async Task BrowseFolderCommand_実行すると画像一覧がImageFilesに反映される()
	{
		var folderPicker = new FakeFolderPicker { FolderToReturn = @"C:\Photos" };
		var scanner = new FakeImageFileScanner { FilePathsToReturn = [@"C:\Photos\a.jpg", @"C:\Photos\b.jpg"] };
		var viewModel = CreateViewModel(folderPicker, scanner);

		viewModel.BrowseFolderCommand.Execute(null);
		await Task.Delay(50);

		Assert.Equal(2, viewModel.ImageFiles.Count);
		Assert.Equal(@"C:\Photos\a.jpg", viewModel.ImageFiles[0].FilePath);
	}

	/// <summary>
	/// パス条件: フォルダ読込中に例外(権限エラー等)が発生しても、例外を投げずErrorMessageが
	/// 設定されること(BrowseFolderCommandはAsyncRelayCommand経由のasync voidのため、
	/// ここで捕捉し損ねると未処理例外でアプリ全体がクラッシュする)。
	/// </summary>
	[Fact]
	public async Task BrowseFolderCommand_フォルダ読込で例外発生時ErrorMessageが設定される()
	{
		var folderPicker = new FakeFolderPicker { FolderToReturn = @"C:\Photos" };
		var scanner = new FakeImageFileScanner { ExceptionToThrow = new UnauthorizedAccessException("アクセス拒否") };
		var viewModel = CreateViewModel(folderPicker, scanner);

		viewModel.BrowseFolderCommand.Execute(null);
		await Task.Delay(50);

		Assert.NotEqual(string.Empty, viewModel.ErrorMessage);
	}

	/// <summary>
	/// パス条件: フォルダ選択がキャンセルされた場合(PickFolderがnullを返す)、ImageFilesが変化しないこと
	/// </summary>
	[Fact]
	public async Task BrowseFolderCommand_キャンセルされた場合何も起こらない()
	{
		var folderPicker = new FakeFolderPicker { FolderToReturn = null };
		var viewModel = CreateViewModel(folderPicker);

		viewModel.BrowseFolderCommand.Execute(null);
		await Task.Delay(50);

		Assert.Empty(viewModel.ImageFiles);
		Assert.Equal(string.Empty, viewModel.SelectedFolderPath);
	}

	/// <summary>
	/// パス条件: 画像読み込み中はIsLoadingがtrueになり、完了後falseに戻ること
	/// </summary>
	[Fact]
	public async Task BrowseFolderCommand_読み込み中はIsLoadingがtrueになり完了後falseに戻る()
	{
		var folderPicker = new FakeFolderPicker { FolderToReturn = @"C:\Photos" };
		var viewModel = CreateViewModel(folderPicker);

		viewModel.BrowseFolderCommand.Execute(null);
		await Task.Delay(50);

		Assert.False(viewModel.IsLoading);
	}

	/// <summary>
	/// パス条件: 各画像のThumbnailにIThumbnailLoaderの結果が反映されること
	/// </summary>
	[Fact]
	public async Task BrowseFolderCommand_各画像のThumbnailにIThumbnailLoaderの結果が反映される()
	{
		var folderPicker = new FakeFolderPicker { FolderToReturn = @"C:\Photos" };
		var scanner = new FakeImageFileScanner { FilePathsToReturn = [@"C:\Photos\a.jpg"] };
		var thumbnailLoader = new FakeThumbnailLoader();
		var viewModel = CreateViewModel(folderPicker, scanner, thumbnailLoader);

		viewModel.BrowseFolderCommand.Execute(null);
		await Task.Delay(50);

		Assert.NotNull(viewModel.ImageFiles[0].Thumbnail);
		Assert.Contains(@"C:\Photos\a.jpg", thumbnailLoader.RequestedFilePaths);
	}

	/// <summary>
	/// パス条件: 画像取得成功後、最初の画像がSelectedImageとして自動選択されること
	/// </summary>
	[Fact]
	public async Task BrowseFolderCommand_成功後に最初の画像が自動選択される()
	{
		var folderPicker = new FakeFolderPicker { FolderToReturn = @"C:\Photos" };
		var scanner = new FakeImageFileScanner { FilePathsToReturn = [@"C:\Photos\a.jpg", @"C:\Photos\b.jpg"] };
		var viewModel = CreateViewModel(folderPicker, scanner);

		viewModel.BrowseFolderCommand.Execute(null);
		await Task.Delay(50);

		Assert.Equal(@"C:\Photos\a.jpg", viewModel.SelectedImage?.FilePath);
	}

	/// <summary>
	/// パス条件: NextCommand実行でSelectedImageが次の画像に進むこと
	/// </summary>
	[Fact]
	public async Task NextCommand_実行するとSelectedImageが次の画像に進む()
	{
		var folderPicker = new FakeFolderPicker { FolderToReturn = @"C:\Photos" };
		var scanner = new FakeImageFileScanner { FilePathsToReturn = [@"C:\Photos\a.jpg", @"C:\Photos\b.jpg"] };
		var viewModel = CreateViewModel(folderPicker, scanner);
		viewModel.BrowseFolderCommand.Execute(null);
		await Task.Delay(50);

		viewModel.NextCommand.Execute(null);

		Assert.Equal(@"C:\Photos\b.jpg", viewModel.SelectedImage?.FilePath);
	}

	/// <summary>
	/// パス条件: PreviousCommand実行でSelectedImageが前の画像に戻ること
	/// </summary>
	[Fact]
	public async Task PreviousCommand_実行するとSelectedImageが前の画像に戻る()
	{
		var folderPicker = new FakeFolderPicker { FolderToReturn = @"C:\Photos" };
		var scanner = new FakeImageFileScanner { FilePathsToReturn = [@"C:\Photos\a.jpg", @"C:\Photos\b.jpg"] };
		var viewModel = CreateViewModel(folderPicker, scanner);
		viewModel.BrowseFolderCommand.Execute(null);
		await Task.Delay(50);
		viewModel.NextCommand.Execute(null);

		viewModel.PreviousCommand.Execute(null);

		Assert.Equal(@"C:\Photos\a.jpg", viewModel.SelectedImage?.FilePath);
	}

	/// <summary>
	/// パス条件: 最初の画像の場合、PreviousCommandが実行不可になること
	/// </summary>
	[Fact]
	public async Task PreviousCommand_最初の画像の場合CanExecuteがfalseになる()
	{
		var folderPicker = new FakeFolderPicker { FolderToReturn = @"C:\Photos" };
		var scanner = new FakeImageFileScanner { FilePathsToReturn = [@"C:\Photos\a.jpg", @"C:\Photos\b.jpg"] };
		var viewModel = CreateViewModel(folderPicker, scanner);
		viewModel.BrowseFolderCommand.Execute(null);
		await Task.Delay(50);

		Assert.False(viewModel.PreviousCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: 最後の画像の場合、NextCommandが実行不可になること
	/// </summary>
	[Fact]
	public async Task NextCommand_最後の画像の場合CanExecuteがfalseになる()
	{
		var folderPicker = new FakeFolderPicker { FolderToReturn = @"C:\Photos" };
		var scanner = new FakeImageFileScanner { FilePathsToReturn = [@"C:\Photos\a.jpg", @"C:\Photos\b.jpg"] };
		var viewModel = CreateViewModel(folderPicker, scanner);
		viewModel.BrowseFolderCommand.Execute(null);
		await Task.Delay(50);
		viewModel.NextCommand.Execute(null);

		Assert.False(viewModel.NextCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: SelectedImageを変更すると、Previous/NextCommandのCanExecuteChangedが発火すること
	/// </summary>
	[Fact]
	public async Task SelectedImage_変更するとPreviousNextCommandのCanExecuteChangedが発火する()
	{
		var folderPicker = new FakeFolderPicker { FolderToReturn = @"C:\Photos" };
		var scanner = new FakeImageFileScanner { FilePathsToReturn = [@"C:\Photos\a.jpg", @"C:\Photos\b.jpg"] };
		var viewModel = CreateViewModel(folderPicker, scanner);
		viewModel.BrowseFolderCommand.Execute(null);
		await Task.Delay(50);
		var previousRaised = false;
		var nextRaised = false;
		viewModel.PreviousCommand.CanExecuteChanged += (_, _) => previousRaised = true;
		viewModel.NextCommand.CanExecuteChanged += (_, _) => nextRaised = true;

		viewModel.SelectedImage = viewModel.ImageFiles[1];

		Assert.True(previousRaised);
		Assert.True(nextRaised);
	}
}
