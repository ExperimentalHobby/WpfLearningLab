using System.IO;
using System.Security;
using System.Windows.Input;
using SystemTrayUtility.Services;

namespace SystemTrayUtility.ViewModels;

/// <summary>
/// システムトレイ常駐ユーティリティのメインViewModel。
/// タスクトレイ・タイマー等の実際のOS/WPF操作はコードビハインド(View)側が担い、
/// このViewModelは設定値の保持と検証のみを担当する。
/// </summary>
public class MainViewModel : ObservableObject
{
	private readonly IStartupRegistrar _startupRegistrar;
	private string _reminderIntervalInput = "30";
	private string _reminderMessage = "休憩を取りましょう";
	private bool _isReminderEnabled;
	private bool _isStartupEnabled;
	private string _statusText = string.Empty;

	/// <summary>
	/// <see cref="MainViewModel"/>を初期化する。
	/// </summary>
	public MainViewModel(IStartupRegistrar startupRegistrar)
	{
		_startupRegistrar = startupRegistrar;
		try
		{
			_isStartupEnabled = _startupRegistrar.IsRegistered();
		}
		catch (Exception ex) when (ex is UnauthorizedAccessException or SecurityException or IOException)
		{
			_isStartupEnabled = false;
			_statusText = $"スタートアップ登録状態の確認に失敗しました: {ex.Message}";
		}

		TestNotifyCommand = new RelayCommand(() => TestNotifyRequested?.Invoke());
	}

	/// <summary>定期リマインダーの間隔(分)入力値。</summary>
	public string ReminderIntervalInput { get => _reminderIntervalInput; set => SetProperty(ref _reminderIntervalInput, value); }

	/// <summary>定期リマインダーで表示するメッセージ。</summary>
	public string ReminderMessage { get => _reminderMessage; set => SetProperty(ref _reminderMessage, value); }

	/// <summary>定期リマインダーが有効かどうか。</summary>
	public bool IsReminderEnabled { get => _isReminderEnabled; set => SetProperty(ref _isReminderEnabled, value); }

	/// <summary>Windows起動時の自動起動が有効かどうか。設定すると即座にレジストリへ反映される。</summary>
	public bool IsStartupEnabled
	{
		get => _isStartupEnabled;
		set
		{
			if (_isStartupEnabled == value)
			{
				return;
			}

			try
			{
				if (value)
				{
					_startupRegistrar.Register();
				}
				else
				{
					_startupRegistrar.Unregister();
				}
			}
			catch (Exception ex) when (ex is UnauthorizedAccessException or SecurityException or IOException or InvalidOperationException)
			{
				// 登録/解除に失敗した場合、内部状態は変更前のまま維持する。
				// フィールドは変えていないが、双方向バインディングされたチェックボックスの表示を
				// 実際の値へ戻すため、通知だけを発火する。
				StatusText = $"スタートアップ設定の変更に失敗しました: {ex.Message}";
				RaisePropertyChanged();
				return;
			}

			SetProperty(ref _isStartupEnabled, value);
			StatusText = value ? "スタートアップに登録しました。" : "スタートアップ登録を解除しました。";
		}
	}

	/// <summary>直近の操作結果を表示するステータステキスト。</summary>
	public string StatusText { get => _statusText; private set => SetProperty(ref _statusText, value); }

	/// <summary>バルーン通知を今すぐ表示するコマンド。</summary>
	public ICommand TestNotifyCommand { get; }

	/// <summary>
	/// <see cref="TestNotifyCommand"/>実行時に発火する。Viewはこれを受けて実際にバルーン通知を表示する。
	/// </summary>
	public event Action? TestNotifyRequested;
}
