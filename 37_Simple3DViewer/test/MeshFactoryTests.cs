using Simple3DViewer.Services;

namespace Simple3DViewer.Tests;

/// <summary>
/// <see cref="MeshFactory"/>のテスト。
/// </summary>
public class MeshFactoryTests
{
	/// <summary>
	/// パス条件: 立方体は24頂点(6面×4頂点)・12三角形(36インデックス)であること。
	/// </summary>
	[Fact]
	public void CreateCube_24頂点12三角形になる()
	{
		var mesh = MeshFactory.CreateCube();

		Assert.Equal(24, mesh.Positions.Count);
		Assert.Equal(36, mesh.TriangleIndices.Count);
	}

	/// <summary>
	/// パス条件: 球は(分割数+1)^2の頂点数、分割数^2×2の三角形数になること。
	/// </summary>
	[Fact]
	public void CreateSphere_分割数に応じた頂点数三角形数になる()
	{
		var mesh = MeshFactory.CreateSphere(radius: 1, slices: 8, stacks: 8);

		Assert.Equal(81, mesh.Positions.Count); // (8+1) * (8+1)
		Assert.Equal(384, mesh.TriangleIndices.Count); // 8 * 8 * 2 * 3
	}

	/// <summary>
	/// パス条件: 球の全頂点が指定した半径の球面上にあること。
	/// </summary>
	[Fact]
	public void CreateSphere_全頂点が指定半径の球面上にある()
	{
		const double radius = 2.5;
		var mesh = MeshFactory.CreateSphere(radius, slices: 8, stacks: 8);

		foreach (var point in mesh.Positions)
		{
			var distanceFromOrigin = Math.Sqrt((point.X * point.X) + (point.Y * point.Y) + (point.Z * point.Z));
			Assert.Equal(radius, distanceFromOrigin, precision: 6);
		}
	}
}
