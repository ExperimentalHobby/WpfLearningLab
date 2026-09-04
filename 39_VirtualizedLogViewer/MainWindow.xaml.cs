using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VirtualizedLogViewer.ViewModels;

namespace VirtualizedLogViewer;

/// <summary>
/// 大量ログビューア(仮想化)のメイン画面。
/// </summary>
public partial class MainWindow : Window
{
	private readonly MainViewModel _viewModel;

	public MainWindow()
	{
		InitializeComponent();

		_viewModel = new MainViewModel();
		DataContext = _viewModel;
		_viewModel.JumpRequested += OnJumpRequested;
	}

	private void OnJumpRequested(int index)
	{
		if (index < 0 || index >= LogListView.Items.Count)
		{
			return;
		}
		var item = LogListView.Items[index];
		LogListView.ScrollIntoView(item);
	}

	/// <summary>
	/// 現在実体化(仮想化パネルによってUI要素として生成)されている<see cref="ListViewItem"/>の数を数え、表示する。
	/// UI仮想化が効いていれば、この数は表示件数(画面に収まる行数程度)に抑えられ、全件が実体化されることはない。
	/// </summary>
	private void CountRealizedContainers_Click(object sender, RoutedEventArgs e)
	{
		var count = CountVisualDescendants<ListViewItem>(LogListView);
		RealizedCountText.Text = $"実体化コンテナ数: {count:N0} / 表示対象 {LogListView.Items.Count:N0}件";
	}

	private static int CountVisualDescendants<T>(DependencyObject root) where T : DependencyObject
	{
		var count = 0;
		var childCount = VisualTreeHelper.GetChildrenCount(root);
		for (var i = 0; i < childCount; i++)
		{
			var child = VisualTreeHelper.GetChild(root, i);
			if (child is T)
			{
				count++;
			}
			count += CountVisualDescendants<T>(child);
		}
		return count;
	}
}
