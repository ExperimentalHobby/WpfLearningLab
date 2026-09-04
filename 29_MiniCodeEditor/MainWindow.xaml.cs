using System.Windows;
using MiniCodeEditor.Services;
using MiniCodeEditor.ViewModels;

namespace MiniCodeEditor;

/// <summary>
/// 簡易コードエディタアプリのメイン画面。DataContextにMainViewModelを設定するのみで、
/// 表示・操作ロジックはすべてViewModel側に委譲する。
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
		var editorController = new AvalonEditController(Editor);
		DataContext = new MainViewModel(editorController, new Win32FileDialogService(), new FileService());
	}
}
