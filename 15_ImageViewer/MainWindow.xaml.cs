using System.Windows;
using ImageViewer.Services;
using ImageViewer.ViewModels;

namespace ImageViewer;

/// <summary>
/// 画像ビューアのメイン画面。DataContextにMainViewModelを設定するのみで、
/// 表示・操作ロジックはすべてViewModel側に委譲する。
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
		DataContext = new MainViewModel(new Win32FolderPicker(), new ImageFileScanner(), new ThumbnailLoader());
	}
}
