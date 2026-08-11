using System.Windows;
using KanbanTaskManager.ViewModels;

namespace KanbanTaskManager;

/// <summary>
/// カンバンボードのメイン画面。DataContextにMainViewModelを設定するのみで、
/// 表示・操作ロジック(タスクの追加/削除/移動)はすべてViewModel/Behavior側に委譲する。
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
		DataContext = new MainViewModel();
	}
}
