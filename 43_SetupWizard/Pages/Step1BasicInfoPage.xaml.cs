using System.Windows;
using System.Windows.Controls;
using SetupWizard.Models;
using SetupWizard.Services;

namespace SetupWizard.Pages;

/// <summary>
/// ウィザードStep1: 氏名・メールアドレスを入力するページ。
/// </summary>
public partial class Step1BasicInfoPage : Page
{
	private readonly WizardState _state;
	private readonly IWizardSettingsRepository _repository;

	/// <summary>ページを初期化する。</summary>
	/// <param name="state">ウィザード全体で共有する入力状態。</param>
	/// <param name="repository">設定の保存に使うリポジトリ。</param>
	public Step1BasicInfoPage(WizardState state, IWizardSettingsRepository repository)
	{
		InitializeComponent();
		_state = state;
		_repository = repository;
		NameTextBox.Text = _state.Name;
		EmailTextBox.Text = _state.Email;
	}

	private void NextButton_Click(object sender, RoutedEventArgs e)
	{
		_state.Name = NameTextBox.Text;
		_state.Email = EmailTextBox.Text;

		var result = WizardValidationEngine.ValidateStep1(_state);
		if (!result.IsValid)
		{
			ErrorTextBlock.Text = result.ErrorMessage;
			return;
		}

		ErrorTextBlock.Text = string.Empty;
		NavigationService?.Navigate(new Step2DetailPage(_state, _repository));
	}
}
