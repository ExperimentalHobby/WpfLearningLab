using System.Windows.Media.Animation;
using AnimatedDashboard.Models;
using AnimatedDashboard.Services;

namespace AnimatedDashboard.Tests;

/// <summary>
/// <see cref="EasingFunctionFactory"/>のテスト。
/// </summary>
public class EasingFunctionFactoryTests
{
	/// <summary>
	/// パス条件: Linearの場合、DoubleAnimation既定の線形補間を使うためnullを返すこと。
	/// </summary>
	[Fact]
	public void Create_Linearの場合nullを返す()
	{
		Assert.Null(EasingFunctionFactory.Create(EasingType.Linear));
	}

	/// <summary>
	/// パス条件: EaseInの場合、EasingMode.EaseInのQuadraticEaseを返すこと。
	/// </summary>
	[Fact]
	public void Create_EaseInの場合EaseInモードのQuadraticEaseを返す()
	{
		var result = Assert.IsType<QuadraticEase>(EasingFunctionFactory.Create(EasingType.EaseIn));
		Assert.Equal(EasingMode.EaseIn, result.EasingMode);
	}

	/// <summary>
	/// パス条件: Bounceの場合、BounceEaseを返すこと。
	/// </summary>
	[Fact]
	public void Create_Bounceの場合BounceEaseを返す()
	{
		Assert.IsType<BounceEase>(EasingFunctionFactory.Create(EasingType.Bounce));
	}

	/// <summary>
	/// パス条件: Elasticの場合、ElasticEaseを返すこと。
	/// </summary>
	[Fact]
	public void Create_Elasticの場合ElasticEaseを返す()
	{
		Assert.IsType<ElasticEase>(EasingFunctionFactory.Create(EasingType.Elastic));
	}
}
