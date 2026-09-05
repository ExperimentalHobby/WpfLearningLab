using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Windows;
using ClaudeChatClient.Services;
using ClaudeChatClient.ViewModels;

namespace ClaudeChatClient;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	private readonly MainViewModel _viewModel;
	private readonly HttpClient _httpClient = new();

	public MainWindow()
	{
		InitializeComponent();

		var appDataDir = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ClaudeChatClient");
		var apiKeyStore = new FileApiKeyStore(Path.Combine(appDataDir, "apikey.json"));

		_viewModel = new MainViewModel(
			apiKeyStore,
			new AesApiKeyCryptoService(),
			apiKey => new ClaudeApiClient(_httpClient, apiKey));

		_viewModel.PropertyChanged += ViewModel_PropertyChanged;
		MessagesList.ItemsSource = _viewModel.Messages;

		ApiKeyInputPanel.Visibility = _viewModel.IsFirstRun ? Visibility.Visible : Visibility.Collapsed;
		LockTitleText.Text = _viewModel.IsFirstRun
			? "初回セットアップ: マスターパスワードとAPIキーを設定してください"
			: "マスターパスワードを入力してください";

		Closed += (_, _) => _httpClient.Dispose();
	}

	private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		switch (e.PropertyName)
		{
			case nameof(MainViewModel.IsUnlocked):
				LockPanel.Visibility = _viewModel.IsUnlocked ? Visibility.Collapsed : Visibility.Visible;
				ChatPanel.Visibility = _viewModel.IsUnlocked ? Visibility.Visible : Visibility.Collapsed;
				break;
			case nameof(MainViewModel.ErrorMessage):
				LockErrorText.Text = _viewModel.ErrorMessage;
				ChatErrorText.Text = _viewModel.ErrorMessage;
				break;
			case nameof(MainViewModel.IsSending):
				SendButton.IsEnabled = !_viewModel.IsSending;
				CancelButton.IsEnabled = _viewModel.IsSending;
				break;
		}
	}

	private void UnlockButton_Click(object sender, RoutedEventArgs e)
	{
		_viewModel.MasterPasswordInput = MasterPasswordBox.Password;

		if (_viewModel.IsFirstRun)
		{
			_viewModel.ApiKeyInput = ApiKeyPasswordBox.Password;
			_viewModel.SetupCommand.Execute(null);
		}
		else
		{
			_viewModel.UnlockCommand.Execute(null);
		}
	}

	private void SendButton_Click(object sender, RoutedEventArgs e)
	{
		_viewModel.InputText = InputTextBox.Text;
		InputTextBox.Text = string.Empty;
		_viewModel.SendCommand.Execute(null);
	}

	private void CancelButton_Click(object sender, RoutedEventArgs e) => _viewModel.CancelCommand.Execute(null);
}
