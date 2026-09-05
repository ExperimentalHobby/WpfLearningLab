using SetupWizard.Models;

namespace SetupWizard;

/// <summary>
/// ウィザード各ステップの入力検証結果。
/// </summary>
/// <param name="IsValid">検証に成功したかどうか。</param>
/// <param name="ErrorMessage">失敗時のエラーメッセージ。成功時は null。</param>
public record ValidationResult(bool IsValid, string? ErrorMessage);

/// <summary>
/// ウィザードの各ステップの入力を検証するロジックを提供する。
/// </summary>
public class WizardValidationEngine
{
    /// <summary>
    /// Step1(氏名・メールアドレス)の入力を検証する。
    /// </summary>
    public ValidationResult ValidateStep1(WizardState state)
    {
        if (string.IsNullOrWhiteSpace(state.Name))
        {
            return new ValidationResult(false, "氏名を入力してください。");
        }

        if (!IsValidEmail(state.Email))
        {
            return new ValidationResult(false, "メールアドレスの形式が正しくありません。");
        }

        return new ValidationResult(true, null);
    }

    /// <summary>
    /// メールアドレスの簡易形式チェック。「@」の前後に1文字以上あり、
    /// 「@」より後ろに「.」が含まれることを確認する。
    /// </summary>
    private static bool IsValidEmail(string email)
    {
        var atIndex = email.IndexOf('@');
        if (atIndex <= 0 || atIndex == email.Length - 1)
        {
            return false;
        }

        return email.IndexOf('.', atIndex) > atIndex;
    }

    /// <summary>
    /// Step2(部署・通知・コメント)の入力を検証する。
    /// </summary>
    public ValidationResult ValidateStep2(WizardState state)
    {
        if (string.IsNullOrWhiteSpace(state.Department))
        {
            return new ValidationResult(false, "部署を選択してください。");
        }

        return new ValidationResult(true, null);
    }
}
