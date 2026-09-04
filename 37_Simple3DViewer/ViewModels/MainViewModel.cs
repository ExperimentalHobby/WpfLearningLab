using System.Windows.Input;
using Simple3DViewer.Models;

namespace Simple3DViewer.ViewModels;

/// <summary>
/// 簡易3Dモデルビューアのメイン画面ViewModel。
/// カメラの方位角・仰角・距離はマウス操作(View側のイベントハンドラ)から更新される。
/// </summary>
public class MainViewModel : ObservableObject
{
	private double _azimuth;
	private double _elevation = 20;
	private double _distance = 6;
	private Object3DType _objectType = Object3DType.Cube;
	private string _materialColorName = "DodgerBlue";
	private bool _isAutoRotating;

	/// <summary>
	/// <see cref="MainViewModel"/>を初期化する。
	/// </summary>
	public MainViewModel()
	{
		SelectCubeCommand = new RelayCommand(() => ObjectType = Object3DType.Cube);
		SelectSphereCommand = new RelayCommand(() => ObjectType = Object3DType.Sphere);
		SetColorCommand = new RelayCommand<string>(color => MaterialColorName = color ?? MaterialColorName);
	}

	/// <summary>立方体を選択するコマンド。</summary>
	public ICommand SelectCubeCommand { get; }

	/// <summary>球を選択するコマンド。</summary>
	public ICommand SelectSphereCommand { get; }

	/// <summary>マテリアルの色を設定するコマンド(パラメータに色名の文字列を渡す)。</summary>
	public ICommand SetColorCommand { get; }

	/// <summary>カメラの方位角(度)。</summary>
	public double Azimuth { get => _azimuth; set => SetProperty(ref _azimuth, value); }

	/// <summary>カメラの仰角(度)。</summary>
	public double Elevation { get => _elevation; set => SetProperty(ref _elevation, value); }

	/// <summary>カメラの距離。</summary>
	public double Distance { get => _distance; set => SetProperty(ref _distance, value); }

	/// <summary>表示中の3Dオブジェクトの種類。</summary>
	public Object3DType ObjectType { get => _objectType; set => SetProperty(ref _objectType, value); }

	/// <summary>マテリアルの色名(<see cref="System.Windows.Media.Colors"/>のプロパティ名)。</summary>
	public string MaterialColorName { get => _materialColorName; set => SetProperty(ref _materialColorName, value); }

	/// <summary>自動回転が有効かどうか。</summary>
	public bool IsAutoRotating { get => _isAutoRotating; set => SetProperty(ref _isAutoRotating, value); }
}
