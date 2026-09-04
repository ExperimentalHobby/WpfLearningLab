using System.Windows;
using AnimatedDashboard.Services;
using AnimatedDashboard.ViewModels;

namespace AnimatedDashboard;

/// <summary>
/// アニメーションダッシュボードのメイン画面。
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
		DataContext = new MainViewModel(new DummyMetricGenerator(new Random()));
	}
}
