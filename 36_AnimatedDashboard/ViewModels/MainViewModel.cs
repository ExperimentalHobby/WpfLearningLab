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
	private readonly IMetricGenerator _generator;
	private EasingType _selectedEasing = EasingType.EaseOut;

	/// <summary>
	/// <see cref="MainViewModel"/>を初期化する。
	/// </summary>
	public MainViewModel(IMetricGenerator generator)
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

		// 共通するインデックス分はValueを更新するだけに留め、KpiCard側のカウントアップ
		// アニメーションが引き続き効くようにする(カードを作り直すとアニメーションが途切れる)。
		var commonCount = Math.Min(Metrics.Count, newMetrics.Count);
		for (var i = 0; i < commonCount; i++)
		{
			Metrics[i].Value = newMetrics[i].Value;
		}

		// 件数が増えた場合は末尾に追加、減った場合は末尾を削除し、件数変化時に
		// 一部の指標が表示されないまま取り残されることがないようにする。
		for (var i = commonCount; i < newMetrics.Count; i++)
		{
			Metrics.Add(new KpiCardViewModel(newMetrics[i].Name, newMetrics[i].Unit, newMetrics[i].Value));
		}
		while (Metrics.Count > newMetrics.Count)
		{
			Metrics.RemoveAt(Metrics.Count - 1);
		}
	}
}
