namespace GaugeControlLib;

/// <summary>
/// <see cref="GaugeControl"/>の角度計算・しきい値判定を担う純粋なロジック。
/// WPFのDispatcher/アニメーションに依存しないため、決定的に単体テストできる。
/// </summary>
public static class GaugeMath
{
	/// <summary>Minimumに対応する針の角度(度)。</summary>
	public const double StartAngle = -90;

	/// <summary>Maximumに対応する針の角度(度)。</summary>
	public const double EndAngle = 90;

	/// <summary>
	/// 値をゲージの針の角度(度)に変換する。<paramref name="value"/>は<paramref name="minimum"/>〜<paramref name="maximum"/>にクランプされる。
	/// </summary>
	public static double ValueToAngle(double value, double minimum, double maximum)
	{
		if (maximum <= minimum)
		{
			return StartAngle;
		}

		var clamped = Math.Clamp(value, minimum, maximum);
		var ratio = (clamped - minimum) / (maximum - minimum);
		return StartAngle + (ratio * (EndAngle - StartAngle));
	}

	/// <summary>
	/// 値が下から上へしきい値を超えて変化したかどうかを判定する。
	/// (既にしきい値以上だった状態からの変化は再度trueにならない)
	/// </summary>
	public static bool HasCrossedThresholdUpward(double oldValue, double newValue, double threshold)
		=> oldValue < threshold && newValue >= threshold;
}
