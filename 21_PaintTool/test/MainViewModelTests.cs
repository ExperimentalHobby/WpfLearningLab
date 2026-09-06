using System.IO;
using System.Windows.Media;
using PaintTool.ViewModels;

namespace PaintTool.Tests;

/// <summary>
/// <see cref="MainViewModel"/> の単体テスト。フェイクの<see cref="FakeInkCanvasController"/>/
/// <see cref="FakeSaveFileDialogService"/>で検証する。
/// </summary>
public class MainViewModelTests
{
	private static MainViewModel CreateViewModel(
		FakeInkCanvasController? controller = null,
		FakeSaveFileDialogService? saveDialog = null) =>
		new(controller ?? new FakeInkCanvasController(), saveDialog ?? new FakeSaveFileDialogService());

	/// <summary>
	/// パス条件: PenColorを変更するとcontroller.SetPenColorが呼ばれること
	/// </summary>
	[Fact]
	public void PenColor_変更するとcontrollerのSetPenColorが呼ばれる()
	{
		var controller = new FakeInkCanvasController();
		var viewModel = CreateViewModel(controller);

		viewModel.PenColor = Colors.Red;

		Assert.Equal(Colors.Red, controller.LastPenColor);
	}

	/// <summary>
	/// パス条件: PenWidthを変更するとcontroller.SetPenWidthが呼ばれること
	/// </summary>
	[Fact]
	public void PenWidth_変更するとcontrollerのSetPenWidthが呼ばれる()
	{
		var controller = new FakeInkCanvasController();
		var viewModel = CreateViewModel(controller);

		viewModel.PenWidth = 10.0;

		Assert.Equal(10.0, controller.LastPenWidth);
	}

	/// <summary>
	/// パス条件: IsEraserModeを変更するとcontroller.SetEraserModeが呼ばれること
	/// </summary>
	[Fact]
	public void IsEraserMode_変更するとcontrollerのSetEraserModeが呼ばれる()
	{
		var controller = new FakeInkCanvasController();
		var viewModel = CreateViewModel(controller);

		viewModel.IsEraserMode = true;

		Assert.True(controller.LastEraserMode);
	}

	/// <summary>
	/// パス条件: UndoCommand実行でcontroller.Undoが呼ばれること
	/// </summary>
	[Fact]
	public void UndoCommand_実行するとcontrollerのUndoが呼ばれる()
	{
		var controller = new FakeInkCanvasController { CanUndo = true };
		var viewModel = CreateViewModel(controller);

		viewModel.UndoCommand.Execute(null);

		Assert.Equal(1, controller.UndoCallCount);
	}

	/// <summary>
	/// パス条件: UndoCommandのCanExecuteがcontroller.CanUndoを反映すること
	/// </summary>
	[Theory]
	[InlineData(true, true)]
	[InlineData(false, false)]
	public void UndoCommand_CanExecuteがcontrollerのCanUndoを反映する(bool canUndo, bool expected)
	{
		var controller = new FakeInkCanvasController { CanUndo = canUndo };
		var viewModel = CreateViewModel(controller);

		Assert.Equal(expected, viewModel.UndoCommand.CanExecute(null));
	}

	/// <summary>
	/// パス条件: RedoCommand実行でcontroller.Redoが呼ばれ、CanExecuteがcontroller.CanRedoを反映すること
	/// </summary>
	[Fact]
	public void RedoCommand_実行するとcontrollerのRedoが呼ばれCanExecuteが反映される()
	{
		var controller = new FakeInkCanvasController { CanRedo = true };
		var viewModel = CreateViewModel(controller);

		Assert.True(viewModel.RedoCommand.CanExecute(null));
		viewModel.RedoCommand.Execute(null);

		Assert.Equal(1, controller.RedoCallCount);
	}

	/// <summary>
	/// パス条件: controllerのStateChangedが発火するとUndo/RedoCommandのCanExecuteChangedが発火すること
	/// </summary>
	[Fact]
	public void controllerのStateChangedでCanExecuteChangedが発火する()
	{
		var controller = new FakeInkCanvasController();
		var viewModel = CreateViewModel(controller);
		var undoRaised = false;
		var redoRaised = false;
		viewModel.UndoCommand.CanExecuteChanged += (_, _) => undoRaised = true;
		viewModel.RedoCommand.CanExecuteChanged += (_, _) => redoRaised = true;

		controller.CanUndo = true;

		Assert.True(undoRaised);
		Assert.True(redoRaised);
	}

	/// <summary>
	/// パス条件: ClearAllCommand実行でcontroller.ClearAllが呼ばれること
	/// </summary>
	[Fact]
	public void ClearAllCommand_実行するとcontrollerのClearAllが呼ばれる()
	{
		var controller = new FakeInkCanvasController();
		var viewModel = CreateViewModel(controller);

		viewModel.ClearAllCommand.Execute(null);

		Assert.Equal(1, controller.ClearAllCallCount);
	}

	/// <summary>
	/// パス条件: SaveCommand実行時、ダイアログでキャンセルされた場合controller.SaveAsPngが呼ばれないこと
	/// </summary>
	[Fact]
	public void SaveCommand_キャンセル時はcontrollerのSaveAsPngが呼ばれない()
	{
		var controller = new FakeInkCanvasController();
		var saveDialog = new FakeSaveFileDialogService { PathToReturn = null };
		var viewModel = CreateViewModel(controller, saveDialog);

		viewModel.SaveCommand.Execute(null);

		Assert.Null(controller.LastSavedPath);
	}

	/// <summary>
	/// パス条件: SaveCommand実行時、ダイアログでパスが選択された場合controller.SaveAsPng(path)が呼ばれること
	/// </summary>
	[Fact]
	public void SaveCommand_パス選択時はcontrollerのSaveAsPngが呼ばれる()
	{
		var controller = new FakeInkCanvasController();
		var saveDialog = new FakeSaveFileDialogService { PathToReturn = @"C:\temp\drawing.png" };
		var viewModel = CreateViewModel(controller, saveDialog);

		viewModel.SaveCommand.Execute(null);

		Assert.Equal(@"C:\temp\drawing.png", controller.LastSavedPath);
	}

	/// <summary>
	/// パス条件: SelectColorCommand実行で色名を解決してPenColorに反映されること
	/// </summary>
	[Fact]
	public void SelectColorCommand_色名を解決してPenColorに反映される()
	{
		var controller = new FakeInkCanvasController();
		var viewModel = CreateViewModel(controller);

		viewModel.SelectColorCommand.Execute("Red");

		Assert.Equal(Colors.Red, viewModel.PenColor);
		Assert.Equal(Colors.Red, controller.LastPenColor);
	}

	/// <summary>
	/// パス条件: SelectColorCommandに不正な色名を渡してもクラッシュせずPenColorが変化しないこと
	/// </summary>
	[Fact]
	public void SelectColorCommand_不正な色名を渡してもクラッシュせずPenColorが変化しない()
	{
		var controller = new FakeInkCanvasController();
		var viewModel = CreateViewModel(controller);
		var originalColor = viewModel.PenColor;

		viewModel.SelectColorCommand.Execute("そんな色はない");

		Assert.Equal(originalColor, viewModel.PenColor);
		Assert.False(string.IsNullOrEmpty(viewModel.ErrorMessage));
	}

	/// <summary>
	/// パス条件: SaveCommand実行時にIOException/UnauthorizedAccessException/InvalidOperationExceptionが
	/// 発生してもクラッシュせずErrorMessageが設定されること
	/// </summary>
	[Theory]
	[InlineData(typeof(IOException))]
	[InlineData(typeof(UnauthorizedAccessException))]
	[InlineData(typeof(InvalidOperationException))]
	public void SaveCommand_保存時に例外が発生してもクラッシュせずErrorMessageが設定される(Type exceptionType)
	{
		var exception = (Exception)Activator.CreateInstance(exceptionType)!;
		var controller = new FakeInkCanvasController { SaveExceptionToThrow = exception };
		var saveDialog = new FakeSaveFileDialogService { PathToReturn = @"C:\temp\drawing.png" };
		var viewModel = CreateViewModel(controller, saveDialog);

		viewModel.SaveCommand.Execute(null);

		Assert.False(string.IsNullOrEmpty(viewModel.ErrorMessage));
	}

	/// <summary>
	/// パス条件: SaveCommandが成功するとErrorMessageがクリアされること
	/// </summary>
	[Fact]
	public void SaveCommand_成功時はErrorMessageがクリアされる()
	{
		var controller = new FakeInkCanvasController { SaveExceptionToThrow = new IOException() };
		var saveDialog = new FakeSaveFileDialogService { PathToReturn = @"C:\temp\drawing.png" };
		var viewModel = CreateViewModel(controller, saveDialog);
		viewModel.SaveCommand.Execute(null);
		Assert.False(string.IsNullOrEmpty(viewModel.ErrorMessage));

		controller.SaveExceptionToThrow = null;
		viewModel.SaveCommand.Execute(null);

		Assert.True(string.IsNullOrEmpty(viewModel.ErrorMessage));
	}
}
