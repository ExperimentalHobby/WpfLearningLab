using System.Windows;
using CustomGaugeControl.ViewModels;

namespace CustomGaugeControl;

/// <summary>
/// カスタムゲージコントロールのデモ画面。
/// </summary>
public partial class MainWindow : Window
{
	private readonly MainViewModel _viewModel = new();

	public MainWindow()
	{
		InitializeComponent();
		DataContext = _viewModel;
	}

	private void TemperatureGauge_ThresholdExceeded(object sender, RoutedEventArgs e)
		=> _viewModel.AddLog("温度がしきい値(80℃)を超えました");

	private void CpuGauge_ThresholdExceeded(object sender, RoutedEventArgs e)
		=> _viewModel.AddLog("CPU使用コア数がしきい値(8)を超えました");
}
