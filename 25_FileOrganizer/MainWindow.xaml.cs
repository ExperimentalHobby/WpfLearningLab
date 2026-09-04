using System.Windows;
using FileOrganizer.Services;
using FileOrganizer.ViewModels;

namespace FileOrganizer;

/// <summary>
/// ファイル自動整理アプリのメイン画面。DataContextにMainViewModelを設定するのみで、
/// 表示・操作ロジックはすべてViewModel側に委譲する。
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
		DataContext = new MainViewModel(
			new FileOrganizerService(),
			new FileSystemDirectoryWatcher(),
			new Win32FolderPicker(),
			new WpfUiDispatcher());
	}
}
