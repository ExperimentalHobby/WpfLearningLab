using GaugeControlLib;

namespace GaugeControlLib.Tests;

/// <summary>
/// <see cref="GaugeMath"/>のテスト。WPFのDispatcher/アニメーションに依存しない純粋なロジックを検証する。
/// </summary>
public class GaugeMathTests
{
	/// <summary>
	/// パス条件: 値がMinimumのとき、開始角度(-90度)になること。
	/// </summary>
	[Fact]
	public void ValueToAngle_値がMinimumのとき開始角度になる()
	{
		Assert.Equal(-90, GaugeMath.ValueToAngle(0, 0, 100));
	}

	/// <summary>
	/// パス条件: 値がMaximumのとき、終了角度(90度)になること。
	/// </summary>
	[Fact]
	public void ValueToAngle_値がMaximumのとき終了角度になる()
	{
		Assert.Equal(90, GaugeMath.ValueToAngle(100, 0, 100));
	}

	/// <summary>
	/// パス条件: 値が中間値のとき、角度が線形補間されること。
	/// </summary>
	[Fact]
	public void ValueToAngle_中間値のとき角度が線形補間される()
	{
		Assert.Equal(0, GaugeMath.ValueToAngle(50, 0, 100));
		Assert.Equal(-45, GaugeMath.ValueToAngle(25, 0, 100));
	}

	/// <summary>
	/// パス条件: 範囲外の値(Maximum超過・Minimum未満)はクランプされること。
	/// </summary>
	[Fact]
	public void ValueToAngle_範囲外の値はクランプされる()
	{
		Assert.Equal(90, GaugeMath.ValueToAngle(150, 0, 100));
		Assert.Equal(-90, GaugeMath.ValueToAngle(-50, 0, 100));
	}

	/// <summary>
	/// パス条件: MaximumがMinimum以下の不正な範囲の場合、開始角度を返すこと(ゼロ除算を回避する)。
	/// </summary>
	[Fact]
	public void ValueToAngle_MaximumがMinimum以下の場合開始角度を返す()
	{
		Assert.Equal(GaugeMath.StartAngle, GaugeMath.ValueToAngle(50, 100, 0));
	}

	/// <summary>
	/// パス条件: 値が下から上へしきい値を超えた場合、trueを返すこと。
	/// </summary>
	[Fact]
	public void HasCrossedThresholdUpward_下から上に超えた場合true()
	{
		Assert.True(GaugeMath.HasCrossedThresholdUpward(oldValue: 70, newValue: 90, threshold: 80));
	}

	/// <summary>
	/// パス条件: 既にしきい値以上だった場合、再度trueにはならないこと。
	/// </summary>
	[Fact]
	public void HasCrossedThresholdUpward_既にしきい値以上の場合false()
	{
		Assert.False(GaugeMath.HasCrossedThresholdUpward(oldValue: 85, newValue: 90, threshold: 80));
	}

	/// <summary>
	/// パス条件: 値がしきい値未満のまま変化した場合、falseを返すこと。
	/// </summary>
	[Fact]
	public void HasCrossedThresholdUpward_しきい値未満のままの場合false()
	{
		Assert.False(GaugeMath.HasCrossedThresholdUpward(oldValue: 10, newValue: 20, threshold: 80));
	}
}
