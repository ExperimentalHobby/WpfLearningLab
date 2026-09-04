using System.Windows.Media.Animation;
using AnimatedDashboard.Models;

namespace AnimatedDashboard.Services;

/// <summary>
/// <see cref="EasingType"/>から実際の<see cref="IEasingFunction"/>を生成するファクトリ。
/// </summary>
public static class EasingFunctionFactory
{
	/// <summary>
	/// 指定した<see cref="EasingType"/>に対応する<see cref="IEasingFunction"/>を生成する。
	/// <see cref="EasingType.Linear"/>の場合は、<see cref="DoubleAnimation"/>既定の線形補間を使うため<see langword="null"/>を返す。
	/// </summary>
	public static IEasingFunction? Create(EasingType type) => type switch
	{
		EasingType.Linear => null,
		EasingType.EaseIn => new QuadraticEase { EasingMode = EasingMode.EaseIn },
		EasingType.EaseOut => new QuadraticEase { EasingMode = EasingMode.EaseOut },
		EasingType.Bounce => new BounceEase { EasingMode = EasingMode.EaseOut, Bounces = 3 },
		EasingType.Elastic => new ElasticEase { EasingMode = EasingMode.EaseOut },
		_ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
	};
}
