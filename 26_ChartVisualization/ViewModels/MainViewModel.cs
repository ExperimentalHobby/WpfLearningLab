using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using ChartVisualization.Models;
using ChartVisualization.Services;
using OxyPlot;
using DataPoint = ChartVisualization.Models.DataPoint;

namespace ChartVisualization.ViewModels;

/// <summary>
/// データ点の入力・グラフ種類の切り替え・グラフ表示を行うメイン画面のViewModel。
/// </summary>
public class MainViewModel : ObservableObject
{
	private string _newLabel = string.Empty;
	private string _newValueInput = string.Empty;
	private ChartType _selectedChartType = ChartType.Bar;

	// OxyPlotの推奨運用に従い、PlotModelインスタンス自体は差し替えずに保持し、内容が変わったら
	// Rebuild + InvalidatePlot(true)で再描画をトリガーする(毎回new PlotModel()すると
	// PlotViewとのバインディング更新のたびに再生成コストがかかる)。
	private readonly PlotModel _plotModel = new() { Title = "データ可視化" };

	/// <summary>
	/// ViewModelを初期化する。
	/// </summary>
	public MainViewModel()
	{
		AddDataPointCommand = new RelayCommand(AddDataPoint, CanAddDataPoint);
		RemoveDataPointCommand = new RelayCommand<DataPoint>(RemoveDataPoint);
	}

	/// <summary>データ点入力フォームのラベル入力欄。</summary>
	public string NewLabel
	{
		get => _newLabel;
		set
		{
			if (SetProperty(ref _newLabel, value))
			{
				AddDataPointCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>データ点入力フォームの値入力欄(数値文字列)。</summary>
	public string NewValueInput
	{
		get => _newValueInput;
		set
		{
			if (SetProperty(ref _newValueInput, value))
			{
				AddDataPointCommand.RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>選択中のグラフ種類。</summary>
	public ChartType SelectedChartType
	{
		get => _selectedChartType;
		set
		{
			if (SetProperty(ref _selectedChartType, value))
			{
				RebuildPlotModel();
			}
		}
	}

	/// <summary>登録済みのデータ点一覧。</summary>
	public ObservableCollection<DataPoint> DataPoints { get; } = [];

	/// <summary>現在のデータ点・グラフ種類から組み立てられた<see cref="PlotModel"/>。インスタンスは固定で、内容のみ更新される。</summary>
	public PlotModel PlotModel => _plotModel;

	/// <summary>入力欄の内容からデータ点を追加するコマンド。</summary>
	public RelayCommand AddDataPointCommand { get; }

	/// <summary>指定したデータ点を削除するコマンド。</summary>
	public RelayCommand<DataPoint> RemoveDataPointCommand { get; }

	private bool CanAddDataPoint() =>
		!string.IsNullOrWhiteSpace(NewLabel) &&
		double.TryParse(NewValueInput, NumberStyles.Float, CultureInfo.InvariantCulture, out _);

	private void AddDataPoint()
	{
		DataPoints.Add(new DataPoint(NewLabel, double.Parse(NewValueInput, CultureInfo.InvariantCulture)));
		NewLabel = string.Empty;
		NewValueInput = string.Empty;
		RebuildPlotModel();
	}

	private void RemoveDataPoint(DataPoint? dataPoint)
	{
		if (dataPoint is not null && DataPoints.Remove(dataPoint))
		{
			RebuildPlotModel();
		}
	}

	private void RebuildPlotModel()
	{
		ChartModelBuilder.Rebuild(_plotModel, DataPoints.ToList(), SelectedChartType);
		_plotModel.InvalidatePlot(true);
	}
}
