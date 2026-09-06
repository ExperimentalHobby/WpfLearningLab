using System.IO;
using System.Windows;
using ContactManager.Data;
using ContactManager.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ContactManager;

/// <summary>
/// 連絡先管理アプリのエントリポイント。
/// 19_HabitTracker等の既存アプリが<c>MainWindow</c>のコンストラクタで手動<c>new</c>していたのに対し、
/// 本アプリは<see cref="Host"/>(Generic Host)+DIコンテナでDbContext/リポジトリ/ViewModel/Windowを解決する。
/// </summary>
public partial class App : Application
{
	private IHost? _host;
	private IServiceScope? _scope;

	/// <inheritdoc/>
	protected override void OnStartup(StartupEventArgs e)
	{
		base.OnStartup(e);

		// AppContext.BaseDirectoryは配置先(Program Files配下等)によっては書き込み権限が
		// ないため、他アプリと同様にApplicationData配下に統一する。
		var dataDirectory = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			"WpfLearningLab.ContactManager");
		Directory.CreateDirectory(dataDirectory);
		var dbPath = Path.Combine(dataDirectory, "contacts.db");

		_host = Host.CreateDefaultBuilder()
			.ConfigureServices(services =>
			{
				services.AddDbContext<ContactManagerDbContext>(options => options.UseSqlite($"Data Source={dbPath}"));
				services.AddScoped<IContactRepository, EfContactRepository>();
				services.AddTransient<MainViewModel>();
				services.AddTransient<MainWindow>();
			})
			.Build();
		_host.Start();

		// デスクトップアプリにはASP.NET Coreのような「1リクエスト=1スコープ」という概念が無いため、
		// アプリのライフタイム全体を1つのDIスコープとして扱う(スコープはOnExitまで保持し破棄する)。
		_scope = _host.Services.CreateScope();
		var context = _scope.ServiceProvider.GetRequiredService<ContactManagerDbContext>();

		try
		{
			context.Database.Migrate();
		}
		catch (Exception ex)
		{
			// DBファイルの破損・権限不足等でマイグレーションに失敗すると、そのまま起動しても
			// 何もできないため、エラーを表示してから終了する。
			MessageBox.Show(
				$"データベースの初期化に失敗しました。\n{ex.Message}",
				"連絡先管理",
				MessageBoxButton.OK,
				MessageBoxImage.Error);
			_scope.Dispose();
			_host.StopAsync().GetAwaiter().GetResult();
			_host.Dispose();
			Shutdown();
			return;
		}

		var mainWindow = _scope.ServiceProvider.GetRequiredService<MainWindow>();
		mainWindow.Show();
	}

	/// <inheritdoc/>
	protected override void OnExit(ExitEventArgs e)
	{
		_scope?.Dispose();
		_host?.StopAsync().GetAwaiter().GetResult();
		_host?.Dispose();
		base.OnExit(e);
	}
}
