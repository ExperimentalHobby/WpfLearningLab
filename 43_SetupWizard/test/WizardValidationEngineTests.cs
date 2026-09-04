using SetupWizard.Models;

namespace SetupWizard.Tests;

/// <summary>
/// <see cref="WizardValidationEngine"/> のテスト。
/// </summary>
public class WizardValidationEngineTests
{
    /// <summary>
    /// パス条件: 名前が空文字の状態でStep1を検証すると失敗すること
    /// </summary>
    [Fact]
    public void ValidateStep1_名前が空の場合はエラーになる()
    {
        var engine = new WizardValidationEngine();
        var state = new WizardState { Name = "", Email = "taro@example.com" };

        var result = engine.ValidateStep1(state);

        Assert.False(result.IsValid);
        Assert.Equal("氏名を入力してください。", result.ErrorMessage);
    }

    /// <summary>
    /// パス条件: メールアドレスが「@」を含まない不正な形式の状態でStep1を検証すると失敗すること
    /// </summary>
    [Fact]
    public void ValidateStep1_メールアドレスの形式が不正な場合はエラーになる()
    {
        var engine = new WizardValidationEngine();
        var state = new WizardState { Name = "山田太郎", Email = "invalid-email" };

        var result = engine.ValidateStep1(state);

        Assert.False(result.IsValid);
        Assert.Equal("メールアドレスの形式が正しくありません。", result.ErrorMessage);
    }

    /// <summary>
    /// パス条件: 氏名・メールアドレスとも正しい形式で入力した状態でStep1を検証すると成功すること
    /// </summary>
    [Fact]
    public void ValidateStep1_正しい入力の場合は成功する()
    {
        var engine = new WizardValidationEngine();
        var state = new WizardState { Name = "山田太郎", Email = "taro@example.com" };

        var result = engine.ValidateStep1(state);

        Assert.True(result.IsValid);
        Assert.Null(result.ErrorMessage);
    }

    /// <summary>
    /// パス条件: 部署が未選択(空文字)の状態でStep2を検証すると失敗すること
    /// </summary>
    [Fact]
    public void ValidateStep2_部署が未選択の場合はエラーになる()
    {
        var engine = new WizardValidationEngine();
        var state = new WizardState { Department = "" };

        var result = engine.ValidateStep2(state);

        Assert.False(result.IsValid);
        Assert.Equal("部署を選択してください。", result.ErrorMessage);
    }

    /// <summary>
    /// パス条件: 部署を選択した状態でStep2を検証すると成功すること
    /// </summary>
    [Fact]
    public void ValidateStep2_正しい入力の場合は成功する()
    {
        var engine = new WizardValidationEngine();
        var state = new WizardState { Department = "開発" };

        var result = engine.ValidateStep2(state);

        Assert.True(result.IsValid);
        Assert.Null(result.ErrorMessage);
    }
}
