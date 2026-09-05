using System.Windows;
using System.Windows.Controls;
using SetupWizard.Models;

namespace SetupWizard.Pages;

/// <summary>
/// ウィザードStep2: 部署・通知設定・コメントを入力するページ。
/// </summary>
public partial class Step2DetailPage : Page
{
    private readonly WizardState _state;
    private readonly WizardValidationEngine _validationEngine = new();

    public Step2DetailPage(WizardState state)
    {
        InitializeComponent();
        _state = state;

        foreach (ComboBoxItem item in DepartmentComboBox.Items)
        {
            if ((string)item.Content == _state.Department)
            {
                DepartmentComboBox.SelectedItem = item;
                break;
            }
        }

        EnableNotificationsCheckBox.IsChecked = _state.EnableNotifications;
        CommentTextBox.Text = _state.Comment;
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        SaveToState();
        NavigationService?.GoBack();
    }

    private void NextButton_Click(object sender, RoutedEventArgs e)
    {
        SaveToState();

        var result = _validationEngine.ValidateStep2(_state);
        if (!result.IsValid)
        {
            ErrorTextBlock.Text = result.ErrorMessage;
            return;
        }

        ErrorTextBlock.Text = string.Empty;
        NavigationService?.Navigate(new Step3ConfirmPage(_state));
    }

    private void SaveToState()
    {
        _state.Department = (DepartmentComboBox.SelectedItem as ComboBoxItem)?.Content as string ?? string.Empty;
        _state.EnableNotifications = EnableNotificationsCheckBox.IsChecked == true;
        _state.Comment = CommentTextBox.Text;
    }
}
