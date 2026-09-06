using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DragDropFileTagger.Data;
using DragDropFileTagger.Models;
using DragDropFileTagger.ViewModels;

namespace DragDropFileTagger;

/// <summary>
/// ドラッグ&amp;ドロップ ファイルタグ付けツールのメイン画面。
/// </summary>
public partial class MainWindow : Window
{
	private readonly MainViewModel _viewModel;
	private Point _dragStartPoint;

	public MainWindow()
	{
		InitializeComponent();

		// AppContext.BaseDirectoryは配置先(Program Files配下等)によっては書き込み権限が
		// ないため、他アプリと同様にApplicationData配下に統一する(33番と同種の対応)。
		var dataDirectory = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			"WpfLearningLab.DragDropFileTagger");
		Directory.CreateDirectory(dataDirectory);
		var savePath = Path.Combine(dataDirectory, "tagged-files.json");
		_viewModel = new MainViewModel(new JsonTaggedFileRepository(savePath));
		DataContext = _viewModel;
	}

	// --- エクスプローラーからのファイルドロップ ---

	private void Window_DragOver(object sender, DragEventArgs e)
	{
		e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
		e.Handled = true;
	}

	private void Window_Drop(object sender, DragEventArgs e)
	{
		if (e.Data.GetDataPresent(DataFormats.FileDrop))
		{
			var paths = (string[])e.Data.GetData(DataFormats.FileDrop)!;
			_viewModel.AddFiles(paths);
		}
	}

	// --- アプリ内でのドラッグ並び替え(DragDrop.DoDragDrop) ---

	private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		_dragStartPoint = e.GetPosition(null);
	}

	private void ListBoxItem_PreviewMouseMove(object sender, MouseEventArgs e)
	{
		if (e.LeftButton != MouseButtonState.Pressed || sender is not ListBoxItem { DataContext: TaggedFile file } item)
		{
			return;
		}

		var current = e.GetPosition(null);
		var diff = _dragStartPoint - current;
		if (Math.Abs(diff.X) < SystemParameters.MinimumHorizontalDragDistance
			&& Math.Abs(diff.Y) < SystemParameters.MinimumVerticalDragDistance)
		{
			return;
		}

		DragDrop.DoDragDrop(item, file, DragDropEffects.Move);
	}

	private void ListBoxItem_Drop(object sender, DragEventArgs e)
	{
		if (sender is not ListBoxItem { DataContext: TaggedFile targetFile }
			|| !e.Data.GetDataPresent(typeof(TaggedFile)))
		{
			return;
		}

		if (e.Data.GetData(typeof(TaggedFile)) is TaggedFile sourceFile && !ReferenceEquals(sourceFile, targetFile))
		{
			_viewModel.MoveFile(sourceFile, targetFile);
		}
		e.Handled = true;
	}
}
