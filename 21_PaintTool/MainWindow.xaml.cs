using System.Windows;
using PaintTool.Services;
using PaintTool.ViewModels;

namespace PaintTool;

/// <summary>
/// お絵かきツールのメイン画面。DataContextにMainViewModelを設定するのみで、
/// 表示・操作ロジックはすべてViewModel(と<see cref="InkCanvasController"/>)側に委譲する。
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();

		var controller = new InkCanvasController(DrawingCanvas);
		var viewModel = new MainViewModel(controller, new Win32SaveFileDialogService());
		DataContext = viewModel;

		// ペンの初期状態(色・太さ)をInkCanvasへ反映する。
		controller.SetPenColor(viewModel.PenColor);
		controller.SetPenWidth(viewModel.PenWidth);

		Closed += (_, _) => controller.Dispose();
	}
}
