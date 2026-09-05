using System.Windows;
using System.Windows.Controls;
using SetupWizard.Models;

namespace SetupWizard.Pages;

/// <summary>
/// ウィザードStep1: 氏名・メールアドレスを入力するページ。
/// </summary>
public partial class Step1BasicInfoPage : Page
{
    private readonly WizardState _state;
    private readonly WizardValidationEngine _validationEngine = new();

    public Step1BasicInfoPage(WizardState state)
    {
        InitializeComponent();
        _state = state;
        NameTextBox.Text = _state.Name;
        EmailTextBox.Text = _state.Email;
    }

    private void NextButton_Click(object sender, RoutedEventArgs e)
    {
        _state.Name = NameTextBox.Text;
        _state.Email = EmailTextBox.Text;

        var result = _validationEngine.ValidateStep1(_state);
        if (!result.IsValid)
        {
            ErrorTextBlock.Text = result.ErrorMessage;
            return;
        }

        ErrorTextBlock.Text = string.Empty;
        NavigationService?.Navigate(new Step2DetailPage(_state));
    }
}
