using System.Windows;
using System.Windows.Controls;
using SetupWizard.Models;

namespace SetupWizard.Pages;

/// <summary>
/// ウィザードStep3: 入力内容の確認・完了/キャンセルを行うページ。
/// </summary>
public partial class Step3ConfirmPage : Page
{
    private readonly WizardState _state;

    public Step3ConfirmPage(WizardState state)
    {
        InitializeComponent();
        _state = state;

        var notifications = _state.EnableNotifications ? "有効" : "無効";
        SummaryTextBlock.Text =
            $"氏名: {_state.Name}\n" +
            $"メールアドレス: {_state.Email}\n" +
            $"部署: {_state.Department}\n" +
            $"通知: {notifications}\n" +
            $"コメント: {_state.Comment}";
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService?.GoBack();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        _state.Reset();
        NavigationService?.Navigate(new Step1BasicInfoPage(_state));
    }

    private void CompleteButton_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("設定が完了しました。", "設定ウィザード", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
