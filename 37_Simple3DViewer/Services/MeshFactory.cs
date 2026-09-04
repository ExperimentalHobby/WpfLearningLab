using System.Windows.Media.Media3D;

namespace Simple3DViewer.Services;

/// <summary>
/// 立方体・球の<see cref="MeshGeometry3D"/>をコード上で生成するファクトリ。
/// </summary>
public static class MeshFactory
{
	/// <summary>
	/// 立方体のメッシュを生成する。面ごとに独立した頂点を持たせ、面ごとの法線(平坦シェーディング)が正しく出るようにする。
	/// </summary>
	public static MeshGeometry3D CreateCube(double size = 1.0)
	{
		var h = size / 2;
		var mesh = new MeshGeometry3D();

		AddFace(mesh, new Point3D(-h, -h, h), new Point3D(h, -h, h), new Point3D(h, h, h), new Point3D(-h, h, h)); // +Z
		AddFace(mesh, new Point3D(h, -h, -h), new Point3D(-h, -h, -h), new Point3D(-h, h, -h), new Point3D(h, h, -h)); // -Z
		AddFace(mesh, new Point3D(-h, -h, -h), new Point3D(-h, -h, h), new Point3D(-h, h, h), new Point3D(-h, h, -h)); // -X
		AddFace(mesh, new Point3D(h, -h, h), new Point3D(h, -h, -h), new Point3D(h, h, -h), new Point3D(h, h, h)); // +X
		AddFace(mesh, new Point3D(-h, h, h), new Point3D(h, h, h), new Point3D(h, h, -h), new Point3D(-h, h, -h)); // +Y
		AddFace(mesh, new Point3D(-h, -h, -h), new Point3D(h, -h, -h), new Point3D(h, -h, h), new Point3D(-h, -h, h)); // -Y

		return mesh;
	}

	/// <summary>
	/// UV球のメッシュを生成する。
	/// </summary>
	/// <param name="radius">半径。</param>
	/// <param name="slices">経度方向の分割数。</param>
	/// <param name="stacks">緯度方向の分割数。</param>
	public static MeshGeometry3D CreateSphere(double radius = 1.0, int slices = 16, int stacks = 16)
	{
		var mesh = new MeshGeometry3D();

		for (var stack = 0; stack <= stacks; stack++)
		{
			var phi = Math.PI * stack / stacks;
			var y = radius * Math.Cos(phi);
			var r = radius * Math.Sin(phi);
			for (var slice = 0; slice <= slices; slice++)
			{
				var theta = 2 * Math.PI * slice / slices;
				var x = r * Math.Cos(theta);
				var z = r * Math.Sin(theta);
				mesh.Positions.Add(new Point3D(x, y, z));
			}
		}

		for (var stack = 0; stack < stacks; stack++)
		{
			for (var slice = 0; slice < slices; slice++)
			{
				var first = (stack * (slices + 1)) + slice;
				var second = first + slices + 1;

				mesh.TriangleIndices.Add(first);
				mesh.TriangleIndices.Add(second);
				mesh.TriangleIndices.Add(first + 1);

				mesh.TriangleIndices.Add(second);
				mesh.TriangleIndices.Add(second + 1);
				mesh.TriangleIndices.Add(first + 1);
			}
		}

		return mesh;
	}

	private static void AddFace(MeshGeometry3D mesh, Point3D p0, Point3D p1, Point3D p2, Point3D p3)
	{
		var baseIndex = mesh.Positions.Count;
		mesh.Positions.Add(p0);
		mesh.Positions.Add(p1);
		mesh.Positions.Add(p2);
		mesh.Positions.Add(p3);

		mesh.TriangleIndices.Add(baseIndex);
		mesh.TriangleIndices.Add(baseIndex + 1);
		mesh.TriangleIndices.Add(baseIndex + 2);

		mesh.TriangleIndices.Add(baseIndex);
		mesh.TriangleIndices.Add(baseIndex + 2);
		mesh.TriangleIndices.Add(baseIndex + 3);
	}
}
