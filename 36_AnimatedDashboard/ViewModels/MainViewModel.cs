using System.Collections.ObjectModel;
using System.Windows.Input;
using AnimatedDashboard.Models;
using AnimatedDashboard.Services;

namespace AnimatedDashboard.ViewModels;

/// <summary>
/// アニメーションダッシュボードのメインViewModel。
/// </summary>
public class MainViewModel : ObservableObject
{
	private readonly DummyMetricGenerator _generator;
	private EasingType _selectedEasing = EasingType.EaseOut;

	/// <summary>
	/// <see cref="MainViewModel"/>を初期化する。
	/// </summary>
	public MainViewModel(DummyMetricGenerator generator)
	{
		_generator = generator;
		RefreshCommand = new RelayCommand(Refresh);

		foreach (var metric in _generator.Generate())
		{
			Metrics.Add(new KpiCardViewModel(metric.Name, metric.Unit, metric.Value));
		}
	}

	/// <summary>表示中のKPIカード一覧。</summary>
	public ObservableCollection<KpiCardViewModel> Metrics { get; } = [];

	/// <summary>選択可能なイージング関数の種類一覧。</summary>
	public IReadOnlyList<EasingType> EasingTypes { get; } = Enum.GetValues<EasingType>();

	/// <summary>カウントアップアニメーションに使うイージング関数。</summary>
	public EasingType SelectedEasing { get => _selectedEasing; set => SetProperty(ref _selectedEasing, value); }

	/// <summary>ダミーデータを再生成し、各KPIカードの値を更新するコマンド。</summary>
	public ICommand RefreshCommand { get; }

	private void Refresh()
	{
		var newMetrics = _generator.Generate();
		for (var i = 0; i < Metrics.Count && i < newMetrics.Count; i++)
		{
			Metrics[i].Value = newMetrics[i].Value;
		}
	}
}
