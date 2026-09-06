using System.Windows;
using System.Windows.Threading;
using MusicPlayer.Services;
using MusicPlayer.ViewModels;

namespace MusicPlayer;

/// <summary>
/// ミュージックプレイヤーのメイン画面。DataContextにMainViewModelを設定し、
/// 実際の<see cref="System.Windows.Controls.MediaElement"/>操作と再生位置のポーリングは
/// View固有の関心事としてこのコードビハインドが担う。
/// </summary>
public partial class MainWindow : Window
{
	private readonly MainViewModel _viewModel;
	private readonly DispatcherTimer _positionTimer = new() { Interval = TimeSpan.FromMilliseconds(250) };

	public MainWindow()
	{
		InitializeComponent();

		var controller = new MediaElementController(Player);
		_viewModel = new MainViewModel(controller, new AudioFileScanner(), new Win32FolderPicker(), new Random());
		DataContext = _viewModel;

		_positionTimer.Tick += (_, _) => _viewModel.ReportPosition(controller.Position);
		_positionTimer.Start();

		Closed += (_, _) => _positionTimer.Stop();
	}
}
