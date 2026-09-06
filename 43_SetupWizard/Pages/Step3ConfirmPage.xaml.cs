using System.IO;
using System.Windows;
using System.Windows.Controls;
using SetupWizard.Models;
using SetupWizard.Services;

namespace SetupWizard.Pages;

/// <summary>
/// ウィザードStep3: 入力内容の確認・完了/キャンセルを行うページ。
/// </summary>
public partial class Step3ConfirmPage : Page
{
	private readonly WizardState _state;
	private readonly IWizardSettingsRepository _repository;

	public Step3ConfirmPage(WizardState state, IWizardSettingsRepository repository)
	{
		InitializeComponent();
		_state = state;
		_repository = repository;

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

		var navigationService = NavigationService;
		navigationService?.Navigate(new Step1BasicInfoPage(_state, _repository));

		// リセット後の新しいStep1から「戻る」を辿ると、キャンセル前のStep3(リセット前の内容)へ
		// 戻れてしまうため、BackStackを空にする。
		while (navigationService?.RemoveBackEntry() is not null)
		{
		}
	}

	private void CompleteButton_Click(object sender, RoutedEventArgs e)
	{
		try
		{
			_repository.Save(_state);
			MessageBox.Show("設定が完了しました。", "設定ウィザード", MessageBoxButton.OK, MessageBoxImage.Information);
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
		{
			MessageBox.Show($"設定の保存に失敗しました: {ex.Message}", "設定ウィザード", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}
}
