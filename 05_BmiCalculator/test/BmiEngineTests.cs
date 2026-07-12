namespace BmiCalculator.Tests;

/// <summary>
/// <see cref="BmiEngine"/> のBMI計算・判定区分に関するテスト。
/// </summary>
public class BmiEngineTests
{
	/// <summary>
	/// パス条件: 身長170cm・体重65kgでBMIが約22.49になること。
	/// </summary>
	[Fact]
	public void CalculateBmi_StandardValues_ReturnsCorrectBmi()
	{
		var engine = new BmiEngine();

		var bmi = engine.CalculateBmi(170m, 65m);

		Assert.Equal(22.49m, Math.Round(bmi, 2));
	}

	/// <summary>
	/// パス条件: BMIが18.4のとき判定区分が「低体重」になること。
	/// </summary>
	[Fact]
	public void JudgeCategory_BelowLowerBoundary_ReturnsUnderweight()
	{
		var engine = new BmiEngine();

		Assert.Equal("低体重", engine.JudgeCategory(18.4m));
	}

	/// <summary>
	/// パス条件: BMIが18.5(境界値、下限を含む)のとき判定区分が「普通体重」になること。
	/// </summary>
	[Fact]
	public void JudgeCategory_AtLowerBoundary_ReturnsNormal()
	{
		var engine = new BmiEngine();

		Assert.Equal("普通体重", engine.JudgeCategory(18.5m));
	}

	/// <summary>
	/// パス条件: BMIが24.9のとき判定区分が「普通体重」になること。
	/// </summary>
	[Fact]
	public void JudgeCategory_JustBelowUpperBoundary_ReturnsNormal()
	{
		var engine = new BmiEngine();

		Assert.Equal("普通体重", engine.JudgeCategory(24.9m));
	}

	/// <summary>
	/// パス条件: BMIが25.0(境界値、肥満の下限を含む)のとき判定区分が「肥満」になること。
	/// </summary>
	[Fact]
	public void JudgeCategory_AtUpperBoundary_ReturnsObese()
	{
		var engine = new BmiEngine();

		Assert.Equal("肥満", engine.JudgeCategory(25.0m));
	}

	/// <summary>
	/// パス条件: BMIが30のとき判定区分が「肥満」になること。
	/// </summary>
	[Fact]
	public void JudgeCategory_HighValue_ReturnsObese()
	{
		var engine = new BmiEngine();

		Assert.Equal("肥満", engine.JudgeCategory(30m));
	}
}
