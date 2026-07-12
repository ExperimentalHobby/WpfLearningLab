namespace BmiCalculator;

/// <summary>
/// 身長・体重からBMI値を計算し、判定区分(低体重/普通体重/肥満)を判定するエンジン。
/// </summary>
public class BmiEngine
{
	/// <summary>
	/// BMI = 体重(kg) / 身長(m)^2 を計算する。
	/// </summary>
	/// <param name="heightCm">身長(cm)。</param>
	/// <param name="weightKg">体重(kg)。</param>
	public decimal CalculateBmi(decimal heightCm, decimal weightKg)
	{
		var heightM = heightCm / 100m;
		return weightKg / (heightM * heightM);
	}

	/// <summary>
	/// BMI値から判定区分(低体重/普通体重/肥満)を判定する。
	/// </summary>
	/// <param name="bmi">判定対象のBMI値。</param>
	public string JudgeCategory(decimal bmi)
	{
		if (bmi < 18.5m)
		{
			return "低体重";
		}

		if (bmi < 25.0m)
		{
			return "普通体重";
		}

		return "肥満";
	}
}
