using System.Collections.ObjectModel;
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
	private PlotModel _plotModel;

	/// <summary>
	/// ViewModelを初期化する。
	/// </summary>
	public MainViewModel()
	{
		AddDataPointCommand = new RelayCommand(AddDataPoint, CanAddDataPoint);
		RemoveDataPointCommand = new RelayCommand<DataPoint>(RemoveDataPoint);
		_plotModel = ChartModelBuilder.Build(DataPoints, SelectedChartType);
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

	/// <summary>現在のデータ点・グラフ種類から組み立てられた<see cref="PlotModel"/>。</summary>
	public PlotModel PlotModel
	{
		get => _plotModel;
		private set => SetProperty(ref _plotModel, value);
	}

	/// <summary>入力欄の内容からデータ点を追加するコマンド。</summary>
	public RelayCommand AddDataPointCommand { get; }

	/// <summary>指定したデータ点を削除するコマンド。</summary>
	public RelayCommand<DataPoint> RemoveDataPointCommand { get; }

	private bool CanAddDataPoint() => !string.IsNullOrWhiteSpace(NewLabel) && double.TryParse(NewValueInput, out _);

	private void AddDataPoint()
	{
		DataPoints.Add(new DataPoint(NewLabel, double.Parse(NewValueInput)));
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

	private void RebuildPlotModel() => PlotModel = ChartModelBuilder.Build(DataPoints.ToList(), SelectedChartType);
}
