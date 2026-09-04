using System.Windows;
using ParallelImageProcessor.Services;
using ParallelImageProcessor.ViewModels;

namespace ParallelImageProcessor;

/// <summary>
/// 並列画像バッチ処理ツールのメイン画面。
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
		DataContext = new MainViewModel(new ImageBatchProcessor(), new Win32FolderPicker());
	}
}
