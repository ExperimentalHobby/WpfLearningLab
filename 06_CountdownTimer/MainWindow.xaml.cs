using System.Media;
using System.Windows;
using System.Windows.Threading;

namespace CountdownTimer;

/// <summary>
/// カウントダウンタイマーのメインウィンドウ。
/// 状態管理は <see cref="CountdownEngine"/> に委譲し、<see cref="DispatcherTimer"/> による1秒ごとのTick呼び出しと
/// 表示更新・通知のみを担当する薄いコードビハインド。
/// </summary>
public partial class MainWindow : Window
{
	private readonly CountdownEngine _engine = new();
	private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(1) };

	/// <summary>
	/// ウィンドウを初期化する。
	/// </summary>
	public MainWindow()
	{
		InitializeComponent();
		_timer.Tick += Timer_Tick;
		UpdateDisplay();
		UpdateButtonStates();
	}

	/// <summary>
	/// 「スタート」ボタン押下時の処理。停止中は入力欄の値を初期時間として設定してから開始し、
	/// 一時停止中はそのまま再開する。
	/// </summary>
	private void StartButton_Click(object sender, RoutedEventArgs e)
	{
		if (_engine.State == CountdownState.Stopped)
		{
			var hours = ParseNonNegativeInt(HourTextBox.Text);
			var minutes = ParseNonNegativeInt(MinuteTextBox.Text);
			var seconds = ParseNonNegativeInt(SecondTextBox.Text);
			_engine.SetInitialTime(new TimeSpan(hours, minutes, seconds));
		}

		_engine.Start();
		if (_engine.State == CountdownState.Running)
		{
			_timer.Start();
		}

		UpdateDisplay();
		UpdateButtonStates();
	}

	/// <summary>
	/// 「一時停止」ボタン押下時の処理。
	/// </summary>
	private void PauseButton_Click(object sender, RoutedEventArgs e)
	{
		_engine.Pause();
		_timer.Stop();
		UpdateButtonStates();
	}

	/// <summary>
	/// 「リセット」ボタン押下時の処理。
	/// </summary>
	private void ResetButton_Click(object sender, RoutedEventArgs e)
	{
		_timer.Stop();
		_engine.Reset();
		UpdateDisplay();
		UpdateButtonStates();
	}

	/// <summary>
	/// 1秒ごとのタイマー処理。残り時間を1秒進め、0に達したら通知する。
	/// </summary>
	private void Timer_Tick(object? sender, EventArgs e)
	{
		var completed = _engine.Tick();
		UpdateDisplay();

		if (completed)
		{
			_timer.Stop();
			UpdateButtonStates();
			SystemSounds.Exclamation.Play();
			MessageBox.Show("タイマーが終了しました。", "カウントダウンタイマー", MessageBoxButton.OK, MessageBoxImage.Information);
		}
	}

	/// <summary>
	/// 入力文字列を0以上の整数として解釈する。数値以外・負の値は0として扱う。
	/// </summary>
	private static int ParseNonNegativeInt(string text)
	{
		return int.TryParse(text, out var value) && value > 0 ? value : 0;
	}

	/// <summary>
	/// 残り時間の表示を更新する。
	/// </summary>
	private void UpdateDisplay()
	{
		TimeDisplayTextBlock.Text = _engine.RemainingTime.ToString(@"hh\:mm\:ss");
	}

	/// <summary>
	/// 現在の状態に応じて入力欄・各ボタンの有効/無効を更新する。
	/// </summary>
	private void UpdateButtonStates()
	{
		var isEditable = _engine.State == CountdownState.Stopped;
		HourTextBox.IsEnabled = isEditable;
		MinuteTextBox.IsEnabled = isEditable;
		SecondTextBox.IsEnabled = isEditable;

		StartButton.IsEnabled = _engine.State is CountdownState.Stopped or CountdownState.Paused;
		PauseButton.IsEnabled = _engine.State == CountdownState.Running;
		ResetButton.IsEnabled = _engine.State != CountdownState.Stopped;
	}
}
