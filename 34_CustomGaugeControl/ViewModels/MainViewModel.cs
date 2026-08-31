using System.Collections.ObjectModel;

namespace CustomGaugeControl.ViewModels;

/// <summary>
/// カスタムゲージコントロールのデモ画面用ViewModel。
/// スライダーで動かす値と、しきい値超過イベントのログを保持する。
/// </summary>
public class MainViewModel : ObservableObject
{
	private double _temperature = 20;
	private double _cpuUsage = 3;

	/// <summary>温度ゲージ(0〜100)の値。</summary>
	public double Temperature { get => _temperature; set => SetProperty(ref _temperature, value); }

	/// <summary>CPU使用率ゲージ(0〜10、10コアを想定した使用コア数のイメージ)の値。</summary>
	public double CpuUsage { get => _cpuUsage; set => SetProperty(ref _cpuUsage, value); }

	/// <summary>しきい値超過イベントのログ。</summary>
	public ObservableCollection<string> EventLog { get; } = [];

	/// <summary>
	/// しきい値超過ログを追加する。
	/// </summary>
	public void AddLog(string message) => EventLog.Insert(0, $"{DateTime.Now:HH:mm:ss} {message}");
}
