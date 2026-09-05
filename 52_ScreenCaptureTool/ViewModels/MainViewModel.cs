using System.Windows.Input;
using System.Windows.Media.Imaging;
using ScreenCaptureTool.Services;

namespace ScreenCaptureTool.ViewModels;

/// <summary>
/// スクリーンキャプチャツールのメイン画面ViewModel。
/// </summary>
public class MainViewModel : ObservableObject
{
	private readonly IScreenCaptureService _captureService;
	private readonly IRegionSelector _regionSelector;
	private readonly IClipboardImageService _clipboardService;
	private readonly IFileSaveService _fileSaveService;
	private readonly ISaveFileDialogService _saveFileDialogService;

	private BitmapSource? _previewImage;
	private string _statusMessage = string.Empty;

	/// <summary>
	/// 直近にキャプチャしたプレビュー画像。
	/// </summary>
	public BitmapSource? PreviewImage
	{
		get => _previewImage;
		private set
		{
			if (SetProperty(ref _previewImage, value))
			{
				(SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
				(CopyCommand as RelayCommand)?.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>
	/// 直近の操作結果を表す状態メッセージ。
	/// </summary>
	public string StatusMessage
	{
		get => _statusMessage;
		private set => SetProperty(ref _statusMessage, value);
	}

	/// <summary>
	/// 仮想デスクトップ全体をキャプチャするコマンド。
	/// </summary>
	public ICommand CaptureFullScreenCommand { get; }

	/// <summary>
	/// マウスドラッグによる範囲選択キャプチャを開始するコマンド。
	/// </summary>
	public ICommand StartRegionSelectCommand { get; }

	/// <summary>
	/// プレビュー画像をファイルに保存するコマンド。
	/// </summary>
	public ICommand SaveCommand { get; }

	/// <summary>
	/// プレビュー画像をクリップボードにコピーするコマンド。
	/// </summary>
	public ICommand CopyCommand { get; }

	/// <summary>
	/// <see cref="MainViewModel"/>を初期化する。
	/// </summary>
	public MainViewModel(
		IScreenCaptureService captureService,
		IRegionSelector regionSelector,
		IClipboardImageService clipboardService,
		IFileSaveService fileSaveService,
		ISaveFileDialogService saveFileDialogService)
	{
		_captureService = captureService;
		_regionSelector = regionSelector;
		_clipboardService = clipboardService;
		_fileSaveService = fileSaveService;
		_saveFileDialogService = saveFileDialogService;

		CaptureFullScreenCommand = new RelayCommand(CaptureFullScreen);
		StartRegionSelectCommand = new RelayCommand(StartRegionSelect);
		SaveCommand = new RelayCommand(Save, () => PreviewImage is not null);
		CopyCommand = new RelayCommand(Copy, () => PreviewImage is not null);
	}

	private void CaptureFullScreen()
	{
		PreviewImage = _captureService.CaptureFullScreen();
		StatusMessage = "全画面をキャプチャしました。";
	}

	private void StartRegionSelect()
	{
		var region = _regionSelector.SelectRegion();
		if (region is null)
		{
			StatusMessage = "範囲選択がキャンセルされました。";
			return;
		}

		PreviewImage = _captureService.CaptureRegion(region);
		StatusMessage = "選択範囲をキャプチャしました。";
	}

	private void Save()
	{
		if (PreviewImage is null)
		{
			return;
		}

		if (!_saveFileDialogService.TryGetSavePath(out var path) || path is null)
		{
			StatusMessage = "保存がキャンセルされました。";
			return;
		}

		_fileSaveService.Save(PreviewImage, path);
		StatusMessage = $"保存しました: {path}";
	}

	private void Copy()
	{
		if (PreviewImage is null)
		{
			return;
		}

		_clipboardService.SetImage(PreviewImage);
		StatusMessage = "クリップボードにコピーしました。";
	}
}
