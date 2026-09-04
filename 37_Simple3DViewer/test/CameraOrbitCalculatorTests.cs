using Simple3DViewer.Services;

namespace Simple3DViewer.Tests;

/// <summary>
/// <see cref="CameraOrbitCalculator"/>のテスト。
/// </summary>
public class CameraOrbitCalculatorTests
{
	/// <summary>
	/// パス条件: 水平方向にドラッグすると、方位角が変化すること。
	/// </summary>
	[Fact]
	public void Drag_水平方向のドラッグで方位角が変化する()
	{
		var (azimuth, elevation) = CameraOrbitCalculator.Drag(azimuth: 0, elevation: 0, deltaX: 10, deltaY: 0, sensitivity: 0.5);

		Assert.Equal(-5, azimuth);
		Assert.Equal(0, elevation);
	}

	/// <summary>
	/// パス条件: 仰角が上限(89度)を超えるドラッグをしても、上限でクランプされること。
	/// </summary>
	[Fact]
	public void Drag_仰角が上限を超えないようクランプされる()
	{
		var (_, elevation) = CameraOrbitCalculator.Drag(azimuth: 0, elevation: 85, deltaX: 0, deltaY: -100, sensitivity: 1);

		Assert.Equal(CameraOrbitCalculator.MaxElevation, elevation);
	}

	/// <summary>
	/// パス条件: 仰角が下限(-89度)を下回るドラッグをしても、下限でクランプされること。
	/// </summary>
	[Fact]
	public void Drag_仰角が下限を下回らないようクランプされる()
	{
		var (_, elevation) = CameraOrbitCalculator.Drag(azimuth: 0, elevation: -85, deltaX: 0, deltaY: 100, sensitivity: 1);

		Assert.Equal(CameraOrbitCalculator.MinElevation, elevation);
	}

	/// <summary>
	/// パス条件: ホイールを前方(正方向)に回すと距離が縮まること。
	/// </summary>
	[Fact]
	public void Zoom_ホイールを正方向に回すと距離が縮まる()
	{
		var distance = CameraOrbitCalculator.Zoom(distance: 10, wheelDelta: 120, sensitivity: 0.01);

		Assert.Equal(8.8, distance, precision: 6);
	}

	/// <summary>
	/// パス条件: 距離が最大値を超えるズームアウトをしても、最大値でクランプされること。
	/// </summary>
	[Fact]
	public void Zoom_距離が最大値を超えないようクランプされる()
	{
		var distance = CameraOrbitCalculator.Zoom(distance: 19, wheelDelta: -1000, sensitivity: 0.01);

		Assert.Equal(CameraOrbitCalculator.MaxDistance, distance);
	}

	/// <summary>
	/// パス条件: 距離が最小値を下回るズームインをしても、最小値でクランプされること。
	/// </summary>
	[Fact]
	public void Zoom_距離が最小値を下回らないようクランプされる()
	{
		var distance = CameraOrbitCalculator.Zoom(distance: 3, wheelDelta: 1000, sensitivity: 0.01);

		Assert.Equal(CameraOrbitCalculator.MinDistance, distance);
	}
}
