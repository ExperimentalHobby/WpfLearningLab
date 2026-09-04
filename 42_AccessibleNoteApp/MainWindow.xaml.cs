using System.IO;
using System.Windows;
using AccessibleNoteApp.Services;
using AccessibleNoteApp.ViewModels;

namespace AccessibleNoteApp;

/// <summary>
/// アクセシブルメモ帳のメイン画面。
/// </summary>
public partial class MainWindow : Window
{
	private readonly MainViewModel _viewModel;

	public MainWindow()
	{
		InitializeComponent();

		var dataDirectory = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			"AccessibleNoteApp");
		_viewModel = new MainViewModel(new JsonMemoRepository(dataDirectory));
		_viewModel.Load();
		DataContext = _viewModel;

		MemoList.ItemActivated += (_, _) => TitleTextBox.Focus();
		MemoList.DeleteRequested += (_, _) => _viewModel.DeleteCommand.Execute(null);
	}
}
