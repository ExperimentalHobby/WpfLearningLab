using System.IO;
using System.Windows;
using HabitTracker.Data;
using HabitTracker.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HabitTracker;

/// <summary>
/// 習慣トラッカーアプリのメイン画面。DataContextにMainViewModelを設定するのみで、
/// 表示・操作ロジックはすべてViewModel側に委譲する。
/// </summary>
public partial class MainWindow : Window
{
	private static readonly string DbPath = Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
		"WpfLearningLab.HabitTracker",
		"habits.db");

	public MainWindow()
	{
		InitializeComponent();

		var directory = Path.GetDirectoryName(DbPath);
		if (directory is not null)
		{
			Directory.CreateDirectory(directory);
		}

		var optionsBuilder = new DbContextOptionsBuilder<HabitTrackerDbContext>();
		optionsBuilder.UseSqlite($"Data Source={DbPath}");
		var context = new HabitTrackerDbContext(optionsBuilder.Options);
		context.Database.Migrate();

		DataContext = new MainViewModel(new EfHabitRepository(context));
	}
}
