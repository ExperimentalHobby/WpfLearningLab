using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using SystemTrayUtility.Services;
using SystemTrayUtility.ViewModels;
using Application = System.Windows.Application;

namespace SystemTrayUtility;

/// <summary>
/// システムトレイ常駐ユーティリティのメイン画面。
/// <see cref="System.Windows.Forms.NotifyIcon"/>(WPFには同等のAPIが無いためWinForms相互運用で使用)で
/// タスクトレイに常駐し、ウィンドウを閉じてもアプリは終了しない。
/// </summary>
public partial class MainWindow : Window
{
	private readonly MainViewModel _viewModel;
	private readonly NotifyIcon _notifyIcon;
	private readonly DispatcherTimer _reminderTimer = new();
	private bool _isExitRequested;

	public MainWindow()
	{
		InitializeComponent();

		var startupRegistrar = new RegistryStartupRegistrar(
			"SystemTrayUtility", Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty);

		_viewModel = new MainViewModel(startupRegistrar);
		DataContext = _viewModel;
		_viewModel.PropertyChanged += ViewModel_PropertyChanged;
		_viewModel.TestNotifyRequested += () => ShowBalloon("テスト通知", _viewModel.ReminderMessage);

		_notifyIcon = new NotifyIcon
		{
			Icon = System.Drawing.SystemIcons.Application,
			Text = "システムトレイ常駐ユーティリティ",
			Visible = true,
			ContextMenuStrip = BuildContextMenu(),
		};
		_notifyIcon.DoubleClick += (_, _) => ShowMainWindow();

		_reminderTimer.Tick += (_, _) => ShowBalloon("定期リマインダー", _viewModel.ReminderMessage);

		// トレイメニューの「終了」以外にも、Windowsのシャットダウン・ログオフ等で
		// Application.Exitが発火する経路があるため、NotifyIconの破棄はここに一元化する。
		Application.Current.Exit += (_, _) => DisposeNotifyIcon();

		Closing += MainWindow_Closing;
	}

	private ContextMenuStrip BuildContextMenu()
	{
		var menu = new ContextMenuStrip();
		menu.Items.Add("開く", null, (_, _) => ShowMainWindow());
		menu.Items.Add("今すぐ通知", null, (_, _) => ShowBalloon("テスト通知", _viewModel.ReminderMessage));
		menu.Items.Add(new ToolStripSeparator());
		menu.Items.Add("終了", null, (_, _) => ExitApplication());
		return menu;
	}

	private void ShowMainWindow()
	{
		Show();
		WindowState = WindowState.Normal;
		Activate();
	}

	private void ShowBalloon(string title, string text)
	{
		_notifyIcon.BalloonTipTitle = title;
		_notifyIcon.BalloonTipText = string.IsNullOrWhiteSpace(text) ? " " : text;
		_notifyIcon.ShowBalloonTip(3000);
	}

	private void MainWindow_Closing(object? sender, CancelEventArgs e)
	{
		if (_isExitRequested)
		{
			return;
		}
		e.Cancel = true;
		Hide();
		ShowBalloon("バックグラウンドで実行中です", "タスクトレイのアイコンから再度開けます。");
	}

	private void ExitApplication()
	{
		_isExitRequested = true;
		Application.Current.Shutdown();
	}

	private void DisposeNotifyIcon()
	{
		_reminderTimer.Stop();
		_notifyIcon.Visible = false;
		_notifyIcon.Dispose();
	}

	private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName is nameof(MainViewModel.IsReminderEnabled) or nameof(MainViewModel.ReminderIntervalInput))
		{
			UpdateReminderTimer();
		}
	}

	private void UpdateReminderTimer()
	{
		_reminderTimer.Stop();
		if (!_viewModel.IsReminderEnabled)
		{
			return;
		}
		if (!ReminderIntervalParser.TryParseMinutes(_viewModel.ReminderIntervalInput, out var interval))
		{
			return;
		}
		_reminderTimer.Interval = interval;
		_reminderTimer.Start();
	}
}
