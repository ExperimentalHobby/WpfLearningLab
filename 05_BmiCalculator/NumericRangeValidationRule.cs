using System.Globalization;
using System.Windows.Controls;

namespace BmiCalculator;

/// <summary>
/// 入力値が数値かつ指定範囲内であることを検証する汎用の <see cref="ValidationRule"/>。
/// 身長・体重など複数の入力欄で <see cref="Min"/>/<see cref="Max"/>/<see cref="FieldName"/> を変えて再利用する。
/// </summary>
public class NumericRangeValidationRule : ValidationRule
{
	/// <summary>
	/// 許容する最小値(この値を含む)。
	/// </summary>
	public decimal Min { get; set; }

	/// <summary>
	/// 許容する最大値(この値を含む)。
	/// </summary>
	public decimal Max { get; set; }

	/// <summary>
	/// エラーメッセージに表示する項目名。
	/// </summary>
	public string FieldName { get; set; } = "値";

	/// <summary>
	/// 入力値を検証する。
	/// </summary>
	/// <param name="value">検証対象の入力値(文字列)。</param>
	/// <param name="cultureInfo">検証に使用するカルチャ。</param>
	public override ValidationResult Validate(object value, CultureInfo cultureInfo)
	{
		var text = value as string ?? string.Empty;

		if (!decimal.TryParse(text, NumberStyles.Float, cultureInfo, out var number))
		{
			return new ValidationResult(false, $"{FieldName}は数値で入力してください。");
		}

		if (number < Min || number > Max)
		{
			return new ValidationResult(false, $"{FieldName}は{Min}〜{Max}の範囲で入力してください。");
		}

		return ValidationResult.ValidResult;
	}
}
