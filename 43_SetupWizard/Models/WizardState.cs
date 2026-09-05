namespace SetupWizard.Models;

/// <summary>
/// ウィザードの各ステップで入力される値を保持する共有状態。
/// MainWindow がインスタンスを1つ保持し、各ページのコンストラクタへ渡す。
/// </summary>
public class WizardState
{
    /// <summary>氏名。</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>メールアドレス。</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>部署。</summary>
    public string Department { get; set; } = string.Empty;

    /// <summary>通知を有効化するかどうか。</summary>
    public bool EnableNotifications { get; set; }

    /// <summary>コメント(任意入力)。</summary>
    public string Comment { get; set; } = string.Empty;

    /// <summary>
    /// 全項目を初期値にリセットする。ウィザードのキャンセル時に使用する。
    /// </summary>
    public void Reset()
    {
        Name = string.Empty;
        Email = string.Empty;
        Department = string.Empty;
        EnableNotifications = false;
        Comment = string.Empty;
    }
}
