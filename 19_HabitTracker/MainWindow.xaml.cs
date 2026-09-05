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

		try
		{
			context.Database.Migrate();
		}
		catch (Exception ex)
		{
			// DB破損・権限エラー等でマイグレーションに失敗すると、WPFの既定の未処理例外
			// ダイアログで唐突に終了してしまう。DBが使えない以上アプリを続行できないため、
			// せめて分かりやすいエラーを表示してから正常終了させる。起動時の致命的エラーで
			// あり復旧手段がないため、ここでは例外の種類を絞らず広く捕捉する。
			MessageBox.Show(
				$"データベースを開始できませんでした。\n{ex.Message}",
				"習慣トラッカー",
				MessageBoxButton.OK,
				MessageBoxImage.Error);
			context.Dispose();
			Application.Current.Shutdown();
			return;
		}

		DataContext = new MainViewModel(new EfHabitRepository(context));
		Closed += (_, _) => context.Dispose();
	}
}
