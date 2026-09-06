using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using Simple3DViewer.Models;
using Simple3DViewer.Services;
using Simple3DViewer.ViewModels;

namespace Simple3DViewer;

/// <summary>
/// 簡易3Dモデルビューアのメイン画面。
/// マウスドラッグでカメラを軌道回転、ホイールでズームする。
/// </summary>
public partial class MainWindow : Window
{
	private readonly MainViewModel _viewModel;
	private readonly DispatcherTimer _autoRotateTimer;
	private Point? _lastMousePosition;

	public MainWindow()
	{
		InitializeComponent();

		_viewModel = new MainViewModel();
		DataContext = _viewModel;
		_viewModel.PropertyChanged += ViewModel_PropertyChanged;

		RebuildObjectVisual();
		UpdateCameraPosition();

		_autoRotateTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
		_autoRotateTimer.Tick += (_, _) => _viewModel.Azimuth += 1;
		// IsAutoRotating=falseの間はタイマー自体を止め、無駄にCPUを使い続けないようにする。
		Closed += (_, _) => _autoRotateTimer.Stop();
	}

	private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		switch (e.PropertyName)
		{
			case nameof(MainViewModel.Azimuth):
			case nameof(MainViewModel.Elevation):
			case nameof(MainViewModel.Distance):
				UpdateCameraPosition();
				break;
			case nameof(MainViewModel.ObjectType):
			case nameof(MainViewModel.MaterialColorName):
				RebuildObjectVisual();
				break;
			case nameof(MainViewModel.IsAutoRotating):
				if (_viewModel.IsAutoRotating)
				{
					_autoRotateTimer.Start();
				}
				else
				{
					_autoRotateTimer.Stop();
				}

				break;
		}
	}

	private void UpdateCameraPosition()
	{
		var azimuthRad = _viewModel.Azimuth * Math.PI / 180;
		var elevationRad = _viewModel.Elevation * Math.PI / 180;
		var distance = _viewModel.Distance;

		var x = distance * Math.Cos(elevationRad) * Math.Sin(azimuthRad);
		var y = distance * Math.Sin(elevationRad);
		var z = distance * Math.Cos(elevationRad) * Math.Cos(azimuthRad);

		Camera.Position = new Point3D(x, y, z);
		Camera.LookDirection = new Vector3D(-x, -y, -z);
	}

	private void RebuildObjectVisual()
	{
		var mesh = _viewModel.ObjectType == Object3DType.Cube
			? MeshFactory.CreateCube(1.5)
			: MeshFactory.CreateSphere(1, 24, 24);

		Color color;
		try
		{
			color = (Color)ColorConverter.ConvertFromString(_viewModel.MaterialColorName)!;
		}
		catch (FormatException)
		{
			// 現状のUIは固定の色名ボタンのみで不正な値を入力する経路はないが、10/21番と同様に
			// 防御的に捕捉する。変換に失敗した場合は前回描画時の色(既定はグレー)を維持する。
			color = Colors.Gray;
		}

		var material = new DiffuseMaterial(new SolidColorBrush(color));
		ObjectVisual.Content = new GeometryModel3D(mesh, material) { BackMaterial = material };
	}

	private void Viewport_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		_lastMousePosition = e.GetPosition(Viewport);
		Viewport.CaptureMouse();
	}

	private void Viewport_MouseMove(object sender, MouseEventArgs e)
	{
		if (_lastMousePosition is null || e.LeftButton != MouseButtonState.Pressed)
		{
			return;
		}

		var current = e.GetPosition(Viewport);
		var deltaX = current.X - _lastMousePosition.Value.X;
		var deltaY = current.Y - _lastMousePosition.Value.Y;
		var (azimuth, elevation) = CameraOrbitCalculator.Drag(_viewModel.Azimuth, _viewModel.Elevation, deltaX, deltaY);
		_viewModel.Azimuth = azimuth;
		_viewModel.Elevation = elevation;
		_lastMousePosition = current;
	}

	private void Viewport_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		_lastMousePosition = null;
		Viewport.ReleaseMouseCapture();
	}

	private void Viewport_MouseWheel(object sender, MouseWheelEventArgs e)
	{
		_viewModel.Distance = CameraOrbitCalculator.Zoom(_viewModel.Distance, e.Delta);
	}
}
