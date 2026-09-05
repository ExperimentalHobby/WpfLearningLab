using System.Collections.ObjectModel;
using System.IO;
using ImageViewer.Services;

namespace ImageViewer.ViewModels;

/// <summary>
/// 画像ビューアのメイン画面のViewModel。フォルダ選択・画像一覧表示・プレビュー切り替えを担う。
/// </summary>
public class MainViewModel : ObservableObject
{
	private readonly IFolderPicker _folderPicker;
	private readonly IImageFileScanner _scanner;
	private readonly IThumbnailLoader _thumbnailLoader;

	private string _selectedFolderPath = string.Empty;
	private bool _isLoading;
	private ImageFileViewModel? _selectedImage;
	private string _errorMessage = string.Empty;

	/// <summary>
	/// ViewModelを初期化する。
	/// </summary>
	public MainViewModel(IFolderPicker folderPicker, IImageFileScanner scanner, IThumbnailLoader thumbnailLoader)
	{
		_folderPicker = folderPicker;
		_scanner = scanner;
		_thumbnailLoader = thumbnailLoader;
		BrowseFolderCommand = new AsyncRelayCommand(BrowseFolderAsync);
		PreviousCommand = new RelayCommand(MoveToPrevious, CanMoveToPrevious);
		NextCommand = new RelayCommand(MoveToNext, CanMoveToNext);
	}

	/// <summary>選択中のフォルダパス。</summary>
	public string SelectedFolderPath
	{
		get => _selectedFolderPath;
		private set => SetProperty(ref _selectedFolderPath, value);
	}

	/// <summary>読み込んだ画像一覧。</summary>
	public ObservableCollection<ImageFileViewModel> ImageFiles { get; } = [];

	/// <summary>読み込み中かどうか。</summary>
	public bool IsLoading
	{
		get => _isLoading;
		private set => SetProperty(ref _isLoading, value);
	}

	/// <summary>エラーメッセージ。エラーが無い場合は空文字。</summary>
	public string ErrorMessage
	{
		get => _errorMessage;
		private set => SetProperty(ref _errorMessage, value);
	}

	/// <summary>プレビュー表示中の画像。</summary>
	public ImageFileViewModel? SelectedImage
	{
		get => _selectedImage;
		set
		{
			if (SetProperty(ref _selectedImage, value))
			{
				PreviousCommand.RaiseCanExecuteChanged();
				NextCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>
	/// フォルダ選択ダイアログを開き、選択されたフォルダの画像一覧を読み込むコマンド。
	/// </summary>
	public AsyncRelayCommand BrowseFolderCommand { get; }

	/// <summary>
	/// 1つ前の画像に切り替えるコマンド。
	/// </summary>
	public RelayCommand PreviousCommand { get; }

	/// <summary>
	/// 1つ次の画像に切り替えるコマンド。
	/// </summary>
	public RelayCommand NextCommand { get; }

	private int SelectedIndex => SelectedImage is null ? -1 : ImageFiles.IndexOf(SelectedImage);

	private bool CanMoveToPrevious() => SelectedIndex > 0;

	private void MoveToPrevious()
	{
		if (CanMoveToPrevious())
		{
			SelectedImage = ImageFiles[SelectedIndex - 1];
		}
	}

	private bool CanMoveToNext() => SelectedIndex >= 0 && SelectedIndex < ImageFiles.Count - 1;

	private void MoveToNext()
	{
		if (CanMoveToNext())
		{
			SelectedImage = ImageFiles[SelectedIndex + 1];
		}
	}

	private async Task BrowseFolderAsync()
	{
		var folder = _folderPicker.PickFolder();
		if (folder is null)
		{
			return;
		}

		SelectedFolderPath = folder;
		IsLoading = true;
		ErrorMessage = string.Empty;
		try
		{
			IReadOnlyList<string> filePaths;
			try
			{
				filePaths = _scanner.GetImageFilePaths(folder);
			}
			catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
			{
				// フォルダへのアクセス権限がない等の理由で列挙に失敗しても、
				// BrowseFolderCommand(AsyncRelayCommand)のExecuteはasync void実装で
				// catchを持たないため、ここで捕捉し損ねると未処理例外でアプリ全体が
				// クラッシュしてしまう。
				ErrorMessage = $"フォルダを読み込めませんでした。\n{ex.Message}";
				return;
			}

			ImageFiles.Clear();
			var imageFileViewModels = filePaths.Select(filePath => new ImageFileViewModel(filePath)).ToList();
			foreach (var imageFile in imageFileViewModels)
			{
				ImageFiles.Add(imageFile);
			}

			SelectedImage = ImageFiles.FirstOrDefault();

			foreach (var imageFile in imageFileViewModels)
			{
				imageFile.Thumbnail = await _thumbnailLoader.LoadAsync(imageFile.FilePath);
			}
		}
		finally
		{
			IsLoading = false;
		}
	}
}
