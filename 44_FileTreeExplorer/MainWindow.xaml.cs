using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FileTreeExplorer.Models;

namespace FileTreeExplorer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly FileSystemBrowserEngine _engine = new(new RealFileSystem());

    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
        KeyDown += MainWindow_KeyDown;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        LoadDriveRoots();
    }

    private void LoadDriveRoots()
    {
        FolderTreeView.Items.Clear();
        foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady))
        {
            FolderTreeView.Items.Add(new FolderNode(drive.Name, drive.RootDirectory.FullName));
        }
    }

    private void FolderTreeView_Expanded(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is not TreeViewItem { DataContext: FolderNode node })
        {
            return;
        }

        if (node.IsLoaded)
        {
            return;
        }

        node.LoadChildren(_engine, out var errorMessage);
        StatusTextBlock.Text = errorMessage ?? string.Empty;
    }

    private void FolderTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (e.NewValue is not FolderNode node)
        {
            return;
        }

        SelectedPathTextBlock.Text = node.FullPath;
        LoadFiles(node.FullPath);
    }

    private void LoadFiles(string path)
    {
        var success = _engine.TryGetFiles(path, out var files, out var errorMessage);
        FilesListView.ItemsSource = success ? files : Array.Empty<FileEntry>();
        StatusTextBlock.Text = errorMessage ?? string.Empty;
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        RefreshSelection();
    }

    private void MainWindow_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.F5)
        {
            RefreshSelection();
        }
    }

    private void RefreshSelection()
    {
        if (FolderTreeView.SelectedItem is not FolderNode node)
        {
            return;
        }

        node.LoadChildren(_engine, out var folderError);
        LoadFiles(node.FullPath);
        if (folderError != null)
        {
            StatusTextBlock.Text = folderError;
        }
    }
}
