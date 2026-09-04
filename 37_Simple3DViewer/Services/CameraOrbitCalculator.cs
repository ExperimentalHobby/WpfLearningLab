namespace Simple3DViewer.Services;

/// <summary>
/// マウスドラッグ/ホイール操作からカメラの方位角・仰角・距離を計算する純粋なロジック。
/// WPFの3D APIに依存しないため、決定的に単体テストできる。
/// </summary>
public static class CameraOrbitCalculator
{
	/// <summary>仰角の最小値(度)。これ未満にはならない(カメラが真下を突き抜けない)。</summary>
	public const double MinElevation = -89;

	/// <summary>仰角の最大値(度)。</summary>
	public const double MaxElevation = 89;

	/// <summary>カメラ距離の最小値。</summary>
	public const double MinDistance = 2;

	/// <summary>カメラ距離の最大値。</summary>
	public const double MaxDistance = 20;

	/// <summary>
	/// マウスドラッグ量から新しい方位角・仰角を計算する。
	/// </summary>
	/// <param name="azimuth">現在の方位角(度)。</param>
	/// <param name="elevation">現在の仰角(度)。</param>
	/// <param name="deltaX">ドラッグの水平移動量(px)。</param>
	/// <param name="deltaY">ドラッグの垂直移動量(px)。</param>
	/// <param name="sensitivity">1pxあたりの角度変化量。</param>
	public static (double Azimuth, double Elevation) Drag(double azimuth, double elevation, double deltaX, double deltaY, double sensitivity = 0.3)
	{
		var newAzimuth = azimuth - (deltaX * sensitivity);
		var newElevation = Math.Clamp(elevation - (deltaY * sensitivity), MinElevation, MaxElevation);
		return (newAzimuth, newElevation);
	}

	/// <summary>
	/// マウスホイール量から新しいカメラ距離を計算する(<see cref="MinDistance"/>〜<see cref="MaxDistance"/>にクランプ)。
	/// </summary>
	/// <param name="distance">現在のカメラ距離。</param>
	/// <param name="wheelDelta">ホイール回転量(<see cref="System.Windows.Input.MouseWheelEventArgs.Delta"/>相当)。</param>
	/// <param name="sensitivity">1あたりの距離変化量。</param>
	public static double Zoom(double distance, double wheelDelta, double sensitivity = 0.01)
		=> Math.Clamp(distance - (wheelDelta * sensitivity), MinDistance, MaxDistance);
}
