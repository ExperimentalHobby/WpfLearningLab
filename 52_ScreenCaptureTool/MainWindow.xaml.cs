using System.Windows;
using ScreenCaptureTool.Services;
using ScreenCaptureTool.ViewModels;

namespace ScreenCaptureTool;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	private readonly MainViewModel _viewModel;

	public MainWindow()
	{
		InitializeComponent();

		var monitorInfoProvider = new Win32MonitorInfoProvider();
		_viewModel = new MainViewModel(
			new GdiScreenCaptureService(monitorInfoProvider),
			new WpfRegionSelector(monitorInfoProvider),
			new WpfClipboardImageService(),
			new PngFileSaveService(),
			new WpfSaveFileDialogService());

		_viewModel.PropertyChanged += (_, _) => UpdateFromViewModel();
		UpdateFromViewModel();
	}

	private void CaptureFullScreenButton_Click(object sender, RoutedEventArgs e) =>
		_viewModel.CaptureFullScreenCommand.Execute(null);

	private void StartRegionSelectButton_Click(object sender, RoutedEventArgs e)
	{
		// キャプチャ対象がオーバーレイに映り込まないよう、選択中は自ウィンドウを一時的に隠す
		Hide();
		try
		{
			_viewModel.StartRegionSelectCommand.Execute(null);
		}
		finally
		{
			Show();
		}
	}

	private void SaveButton_Click(object sender, RoutedEventArgs e) => _viewModel.SaveCommand.Execute(null);

	private void CopyButton_Click(object sender, RoutedEventArgs e) => _viewModel.CopyCommand.Execute(null);

	private void UpdateFromViewModel()
	{
		PreviewImageControl.Source = _viewModel.PreviewImage;
		StatusText.Text = _viewModel.StatusMessage;
		SaveButton.IsEnabled = _viewModel.SaveCommand.CanExecute(null);
		CopyButton.IsEnabled = _viewModel.CopyCommand.CanExecute(null);
	}
}
