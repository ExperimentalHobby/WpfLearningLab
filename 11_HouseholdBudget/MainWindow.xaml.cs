using System.IO;
using System.Windows;
using HouseholdBudget.Data;
using HouseholdBudget.ViewModels;

namespace HouseholdBudget;

/// <summary>
/// 家計簿アプリのメイン画面。DataContextにMainViewModelを設定するのみで、
/// 表示・操作ロジックはすべてViewModel側に委譲する。
/// </summary>
public partial class MainWindow : Window
{
	private static readonly string DbPath = Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
		"WpfLearningLab.HouseholdBudget",
		"budget.db");

	public MainWindow()
	{
		InitializeComponent();

		var directory = Path.GetDirectoryName(DbPath);
		if (directory is not null)
		{
			Directory.CreateDirectory(directory);
		}

		DataContext = new MainViewModel(new SqliteTransactionRepository(DbPath));
	}
}
