using System.Globalization;

namespace BmiCalculator.Tests;

/// <summary>
/// <see cref="NumericRangeValidationRule"/> の数値範囲検証に関するテスト。
/// </summary>
public class NumericRangeValidationRuleTests
{
	/// <summary>
	/// パス条件: 数値以外の文字列を検証すると無効と判定されエラーメッセージが設定されること。
	/// </summary>
	[Fact]
	public void Validate_NonNumericInput_ReturnsInvalid()
	{
		var rule = new NumericRangeValidationRule { Min = 50m, Max = 250m, FieldName = "身長" };

		var result = rule.Validate("abc", CultureInfo.InvariantCulture);

		Assert.False(result.IsValid);
		Assert.Equal("身長は数値で入力してください。", result.ErrorContent);
	}

	/// <summary>
	/// パス条件: 空文字を検証すると無効と判定されエラーメッセージが設定されること。
	/// </summary>
	[Fact]
	public void Validate_EmptyInput_ReturnsInvalid()
	{
		var rule = new NumericRangeValidationRule { Min = 50m, Max = 250m, FieldName = "身長" };

		var result = rule.Validate(string.Empty, CultureInfo.InvariantCulture);

		Assert.False(result.IsValid);
	}

	/// <summary>
	/// パス条件: 最小値未満の数値を検証すると無効と判定されエラーメッセージが設定されること。
	/// </summary>
	[Fact]
	public void Validate_BelowMin_ReturnsInvalid()
	{
		var rule = new NumericRangeValidationRule { Min = 50m, Max = 250m, FieldName = "身長" };

		var result = rule.Validate("49", CultureInfo.InvariantCulture);

		Assert.False(result.IsValid);
		Assert.Equal("身長は50〜250の範囲で入力してください。", result.ErrorContent);
	}

	/// <summary>
	/// パス条件: 最大値超過の数値を検証すると無効と判定されること。
	/// </summary>
	[Fact]
	public void Validate_AboveMax_ReturnsInvalid()
	{
		var rule = new NumericRangeValidationRule { Min = 50m, Max = 250m, FieldName = "身長" };

		var result = rule.Validate("251", CultureInfo.InvariantCulture);

		Assert.False(result.IsValid);
	}

	/// <summary>
	/// パス条件: 最小値ちょうど(境界値)を検証すると有効と判定されること。
	/// </summary>
	[Fact]
	public void Validate_AtMinBoundary_ReturnsValid()
	{
		var rule = new NumericRangeValidationRule { Min = 50m, Max = 250m, FieldName = "身長" };

		var result = rule.Validate("50", CultureInfo.InvariantCulture);

		Assert.True(result.IsValid);
	}

	/// <summary>
	/// パス条件: 最大値ちょうど(境界値)を検証すると有効と判定されること。
	/// </summary>
	[Fact]
	public void Validate_AtMaxBoundary_ReturnsValid()
	{
		var rule = new NumericRangeValidationRule { Min = 50m, Max = 250m, FieldName = "身長" };

		var result = rule.Validate("250", CultureInfo.InvariantCulture);

		Assert.True(result.IsValid);
	}

	/// <summary>
	/// パス条件: 範囲内の通常値を検証すると有効と判定されること。
	/// </summary>
	[Fact]
	public void Validate_WithinRange_ReturnsValid()
	{
		var rule = new NumericRangeValidationRule { Min = 50m, Max = 250m, FieldName = "身長" };

		var result = rule.Validate("170", CultureInfo.InvariantCulture);

		Assert.True(result.IsValid);
	}
}
