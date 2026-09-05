using ScreenCaptureTool.Models;
using ScreenCaptureTool.Tests.Fakes;
using ScreenCaptureTool.ViewModels;

namespace ScreenCaptureTool.Tests;

public class MainViewModelTests
{
	private static (MainViewModel vm, FakeScreenCaptureService capture, FakeRegionSelector selector,
		FakeClipboardImageService clipboard, FakeFileSaveService fileSave, FakeSaveFileDialogService dialog) CreateViewModel()
	{
		var capture = new FakeScreenCaptureService();
		var selector = new FakeRegionSelector();
		var clipboard = new FakeClipboardImageService();
		var fileSave = new FakeFileSaveService();
		var dialog = new FakeSaveFileDialogService();
		var vm = new MainViewModel(capture, selector, clipboard, fileSave, dialog);
		return (vm, capture, selector, clipboard, fileSave, dialog);
	}

	/// <summary>
	/// パス条件: CaptureFullScreenCommand実行でPreviewImageが設定されること。
	/// </summary>
	[Fact]
	public void CaptureFullScreenCommand_SetsPreviewImage()
	{
		var (vm, capture, _, _, _, _) = CreateViewModel();

		vm.CaptureFullScreenCommand.Execute(null);

		Assert.NotNull(vm.PreviewImage);
		Assert.Equal(1, capture.FullScreenCallCount);
	}

	/// <summary>
	/// パス条件: 範囲選択で領域が得られた場合、その領域でCaptureRegionが呼ばれPreviewImageが設定されること。
	/// </summary>
	[Fact]
	public void StartRegionSelectCommand_WithSelectedRegion_CapturesRegion()
	{
		var (vm, capture, selector, _, _, _) = CreateViewModel();
		var region = new CaptureRegion(10, 20, 100, 200);
		selector.ResultToReturn = region;

		vm.StartRegionSelectCommand.Execute(null);

		Assert.NotNull(vm.PreviewImage);
		Assert.Equal(region, capture.LastRequestedRegion);
	}

	/// <summary>
	/// パス条件: 範囲選択がキャンセルされた場合、PreviewImageは変更されずキャンセルメッセージが表示されること。
	/// </summary>
	[Fact]
	public void StartRegionSelectCommand_Cancelled_DoesNotCapture()
	{
		var (vm, capture, selector, _, _, _) = CreateViewModel();
		selector.ResultToReturn = null;

		vm.StartRegionSelectCommand.Execute(null);

		Assert.Null(vm.PreviewImage);
		Assert.Null(capture.LastRequestedRegion);
		Assert.Contains("キャンセル", vm.StatusMessage);
	}

	/// <summary>
	/// パス条件: PreviewImageが無い状態ではSaveCommand/CopyCommandがCanExecute=falseであること。
	/// </summary>
	[Fact]
	public void SaveAndCopyCommands_CanExecuteFalse_WhenNoPreviewImage()
	{
		var (vm, _, _, _, _, _) = CreateViewModel();

		Assert.False(vm.SaveCommand.CanExecute(null));
		Assert.False(vm.CopyCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: SaveCommand実行時、保存先パスが取得できればIFileSaveServiceに保存されること。
	/// </summary>
	[Fact]
	public void SaveCommand_PathProvided_SavesImage()
	{
		var (vm, _, _, _, fileSave, dialog) = CreateViewModel();
		vm.CaptureFullScreenCommand.Execute(null);
		dialog.PathToReturn = @"C:\temp\capture.png";

		vm.SaveCommand.Execute(null);

		Assert.Equal(@"C:\temp\capture.png", fileSave.LastSavedPath);
		Assert.NotNull(fileSave.LastSavedImage);
	}

	/// <summary>
	/// パス条件: SaveCommand実行時、保存先パスがキャンセルされた場合は保存されないこと。
	/// </summary>
	[Fact]
	public void SaveCommand_DialogCancelled_DoesNotSave()
	{
		var (vm, _, _, _, fileSave, dialog) = CreateViewModel();
		vm.CaptureFullScreenCommand.Execute(null);
		dialog.PathToReturn = null;

		vm.SaveCommand.Execute(null);

		Assert.Null(fileSave.LastSavedPath);
	}

	/// <summary>
	/// パス条件: CopyCommand実行でIClipboardImageServiceにPreviewImageが渡ること。
	/// </summary>
	[Fact]
	public void CopyCommand_SetsClipboardImage()
	{
		var (vm, _, _, clipboard, _, _) = CreateViewModel();
		vm.CaptureFullScreenCommand.Execute(null);

		vm.CopyCommand.Execute(null);

		Assert.Same(vm.PreviewImage, clipboard.LastSetImage);
	}
}
