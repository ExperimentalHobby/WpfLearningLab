using System.Windows;
using MemoryLeakLab.ViewModels;

namespace MemoryLeakLab;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	private readonly MainViewModel _viewModel = new();

	public MainWindow()
	{
		InitializeComponent();
		UpdateCounts();
	}

	private void ModeRadio_Checked(object sender, RoutedEventArgs e)
	{
		_viewModel.Mode = BadModeRadio.IsChecked == true ? LeakMode.Bad : LeakMode.Good;
	}

	private void GenerateButton_Click(object sender, RoutedEventArgs e)
	{
		_viewModel.GenerateCommand.Execute(null);
		UpdateCounts();
	}

	private void ReleaseButton_Click(object sender, RoutedEventArgs e)
	{
		_viewModel.ReleaseReferencesCommand.Execute(null);
		UpdateCounts();
	}

	private void CollectGarbageButton_Click(object sender, RoutedEventArgs e)
	{
		_viewModel.CollectGarbageCommand.Execute(null);
		UpdateCounts();
	}

	private void UpdateCounts()
	{
		TotalCountText.Text = _viewModel.TotalCount.ToString();
		AliveCountText.Text = _viewModel.AliveCount.ToString();
	}
}
