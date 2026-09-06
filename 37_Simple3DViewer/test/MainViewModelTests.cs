using Simple3DViewer.Models;
using Simple3DViewer.ViewModels;

namespace Simple3DViewer.Tests;

/// <summary>
/// <see cref="MainViewModel"/>のテスト。
/// </summary>
public class MainViewModelTests
{
	/// <summary>
	/// パス条件: SelectSphereCommandを実行すると、ObjectTypeがSphereになること。
	/// </summary>
	[Fact]
	public void SelectSphereCommand_実行するとObjectTypeがSphereになる()
	{
		var viewModel = new MainViewModel();

		viewModel.SelectSphereCommand.Execute(null);

		Assert.Equal(Object3DType.Sphere, viewModel.ObjectType);
	}

	/// <summary>
	/// パス条件: SelectCubeCommandを実行すると、ObjectTypeがCubeになること。
	/// </summary>
	[Fact]
	public void SelectCubeCommand_実行するとObjectTypeがCubeになる()
	{
		var viewModel = new MainViewModel();
		viewModel.SelectSphereCommand.Execute(null);

		viewModel.SelectCubeCommand.Execute(null);

		Assert.Equal(Object3DType.Cube, viewModel.ObjectType);
	}

	/// <summary>
	/// パス条件: SetColorCommandを実行すると、指定した色名がMaterialColorNameに反映されること。
	/// </summary>
	[Fact]
	public void SetColorCommand_実行すると色名が反映される()
	{
		var viewModel = new MainViewModel();

		viewModel.SetColorCommand.Execute("Crimson");

		Assert.Equal("Crimson", viewModel.MaterialColorName);
	}

	/// <summary>
	/// パス条件: Azimuthに360以上の値を設定すると、[0, 360)の範囲に正規化されること
	/// (自動回転で際限なく加算し続けるとdoubleの精度が劣化するため)。
	/// </summary>
	[Fact]
	public void Azimuth_360以上の値は正規化される()
	{
		var viewModel = new MainViewModel();

		viewModel.Azimuth = 361;

		Assert.Equal(1, viewModel.Azimuth, precision: 10);
	}

	/// <summary>
	/// パス条件: Azimuthに負の値を設定すると、[0, 360)の範囲に正規化されること。
	/// </summary>
	[Fact]
	public void Azimuth_負の値は正規化される()
	{
		var viewModel = new MainViewModel();

		viewModel.Azimuth = -1;

		Assert.Equal(359, viewModel.Azimuth, precision: 10);
	}
}
