using System.ComponentModel;
using System.IO;
using System.Windows;
using PasswordManager.Data;
using PasswordManager.Services;
using PasswordManager.ViewModels;

namespace PasswordManager;

/// <summary>
/// パスワード管理アプリのメイン画面。DataContextにMainViewModelを設定し、
/// <see cref="System.Windows.Controls.PasswordBox"/>はセキュリティ上バインドできないため
/// コードビハインドでViewModelのプロパティと同期する。
/// </summary>
public partial class MainWindow : Window
{
	private static readonly string DbPath = Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
		"WpfLearningLab.PasswordManager",
		"passwords.db");

	private readonly MainViewModel _viewModel;

	public MainWindow()
	{
		InitializeComponent();

		var directory = Path.GetDirectoryName(DbPath);
		if (directory is not null)
		{
			Directory.CreateDirectory(directory);
		}

		var repository = new SqlitePasswordRepository(DbPath);
		_viewModel = new MainViewModel(repository, repository, new AesPasswordCryptoService(), new WpfClipboardService());
		DataContext = _viewModel;

		MasterPasswordBox.PasswordChanged += (_, _) => _viewModel.MasterPasswordInput = MasterPasswordBox.Password;
		MasterPasswordConfirmBox.PasswordChanged += (_, _) => _viewModel.MasterPasswordConfirmInput = MasterPasswordConfirmBox.Password;

		InputPasswordBox.PasswordChanged += (_, _) => _viewModel.InputPassword = InputPasswordBox.Password;
		_viewModel.PropertyChanged += OnViewModelPropertyChanged;
	}

	private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		// InputPasswordはSelectedEntry選択時やクリア時にViewModel側から変更されるため、
		// PasswordBoxへ反映する(同じ値なら無視してイベントの無限ループを防ぐ)。
		if (e.PropertyName == nameof(MainViewModel.InputPassword) && InputPasswordBox.Password != _viewModel.InputPassword)
		{
			InputPasswordBox.Password = _viewModel.InputPassword;
		}
	}
}
