using System.Collections.ObjectModel;
using System.Windows.Input;
using GlobalHotkeyLauncher.Models;
using GlobalHotkeyLauncher.Services;
using Key = System.Windows.Input.Key;

namespace GlobalHotkeyLauncher.ViewModels;

/// <summary>
/// グローバルホットキーの登録・編集・削除と、発火時のコマンド実行を管理するViewModel。
/// </summary>
public class MainViewModel : ObservableObject
{
	private readonly IHotKeyRegistrar _registrar;
	private readonly ICommandLauncher _launcher;
	private int _nextId = 1;

	private bool _isCtrlSelected;
	private bool _isAltSelected;
	private bool _isShiftSelected;
	private bool _isWinSelected;
	private Key _selectedKey = Key.None;
	private string _label = string.Empty;
	private string _target = string.Empty;
	private string? _errorMessage;

	/// <summary>
	/// 登録済みホットキーの一覧・実行ログを空の状態で初期化する。
	/// </summary>
	/// <param name="registrar">OSへのホットキー登録を行う実装。</param>
	/// <param name="launcher">ホットキー発火時にコマンドを実行する実装。</param>
	public MainViewModel(IHotKeyRegistrar registrar, ICommandLauncher launcher)
	{
		_registrar = registrar;
		_launcher = launcher;

		AddHotKeyCommand = new RelayCommand(AddHotKey);
		RemoveHotKeyCommand = new RelayCommand<HotKeyBinding>(RemoveHotKey);
		EditHotKeyCommand = new RelayCommand<HotKeyBinding>(EditHotKey);
	}

	/// <summary>キー選択欄に表示する候補(アルファベット・数字・ファンクションキー)。</summary>
	public IReadOnlyList<Key> AvailableKeys { get; } = BuildAvailableKeys();

	/// <summary>登録済みホットキーの一覧。</summary>
	public ObservableCollection<HotKeyBinding> Bindings { get; } = [];

	/// <summary>ホットキー登録・発火の実行ログ(新しい順)。</summary>
	public ObservableCollection<string> ExecutionLog { get; } = [];

	/// <summary>入力中の組み合わせにCtrlを含めるかどうか。</summary>
	public bool IsCtrlSelected
	{
		get => _isCtrlSelected;
		set => SetProperty(ref _isCtrlSelected, value);
	}

	/// <summary>入力中の組み合わせにAltを含めるかどうか。</summary>
	public bool IsAltSelected
	{
		get => _isAltSelected;
		set => SetProperty(ref _isAltSelected, value);
	}

	/// <summary>入力中の組み合わせにShiftを含めるかどうか。</summary>
	public bool IsShiftSelected
	{
		get => _isShiftSelected;
		set => SetProperty(ref _isShiftSelected, value);
	}

	/// <summary>入力中の組み合わせにWin(Windowsキー)を含めるかどうか。</summary>
	public bool IsWinSelected
	{
		get => _isWinSelected;
		set => SetProperty(ref _isWinSelected, value);
	}

	/// <summary>入力中の組み合わせの通常キー。</summary>
	public Key SelectedKey
	{
		get => _selectedKey;
		set => SetProperty(ref _selectedKey, value);
	}

	/// <summary>登録するホットキーの説明(一覧表示用)。</summary>
	public string Label
	{
		get => _label;
		set => SetProperty(ref _label, value);
	}

	/// <summary>実行対象(実行ファイルのパスまたはURL)。</summary>
	public string Target
	{
		get => _target;
		set => SetProperty(ref _target, value);
	}

	/// <summary>直近の登録操作で発生したエラーメッセージ。エラーが無ければ<see langword="null"/>。</summary>
	public string? ErrorMessage
	{
		get => _errorMessage;
		private set => SetProperty(ref _errorMessage, value);
	}

	/// <summary>入力中の組み合わせでホットキーを登録するコマンド。</summary>
	public ICommand AddHotKeyCommand { get; }

	/// <summary>指定したホットキーの登録を解除するコマンド。</summary>
	public ICommand RemoveHotKeyCommand { get; }

	/// <summary>指定したホットキーを解除し、入力欄に値を復元して編集を開始するコマンド。</summary>
	public ICommand EditHotKeyCommand { get; }

	/// <summary>
	/// <c>WM_HOTKEY</c>受信時に呼び出される。該当するホットキーが登録済みであれば、
	/// 対応するコマンドを実行し実行ログに記録する。未登録のIDの場合は何もしない。
	/// </summary>
	/// <param name="id">発火したホットキーのID。</param>
	public void HandleHotKeyTriggered(int id)
	{
		var binding = Bindings.FirstOrDefault(b => b.Id == id);
		if (binding is null)
		{
			return;
		}

		if (_launcher.Launch(binding.Target))
		{
			ExecutionLog.Insert(0, $"実行: {binding.Label} ({binding.Target})");
		}
		else
		{
			ExecutionLog.Insert(0, $"実行失敗: {binding.Label} ({binding.Target})");
		}
	}

	private void AddHotKey()
	{
		ErrorMessage = null;

		var combination = BuildCombination();
		if (!combination.Validate(out var validationError))
		{
			ErrorMessage = validationError;
			return;
		}
		if (string.IsNullOrWhiteSpace(Label) || string.IsNullOrWhiteSpace(Target))
		{
			ErrorMessage = "説明と実行対象を入力してください。";
			return;
		}
		if (Bindings.Any(b => b.Combination.Equals(combination)))
		{
			ErrorMessage = $"「{combination.ToDisplayString()}」は既に登録されています。";
			return;
		}

		var id = _nextId++;
		if (!_registrar.TryRegister(id, combination))
		{
			ErrorMessage = $"「{combination.ToDisplayString()}」の登録に失敗しました(他のアプリで使用中の可能性があります)。";
			return;
		}

		Bindings.Add(new HotKeyBinding(id, combination, Label, Target));
		ExecutionLog.Insert(0, $"登録: {combination.ToDisplayString()} → {Label}");
		Label = string.Empty;
		Target = string.Empty;
	}

	private void RemoveHotKey(HotKeyBinding? binding)
	{
		if (binding is null)
		{
			return;
		}

		_registrar.Unregister(binding.Id);
		Bindings.Remove(binding);
	}

	private void EditHotKey(HotKeyBinding? binding)
	{
		if (binding is null)
		{
			return;
		}

		_registrar.Unregister(binding.Id);
		Bindings.Remove(binding);

		IsCtrlSelected = binding.Combination.Modifiers.HasFlag(ModifierKeys.Control);
		IsAltSelected = binding.Combination.Modifiers.HasFlag(ModifierKeys.Alt);
		IsShiftSelected = binding.Combination.Modifiers.HasFlag(ModifierKeys.Shift);
		IsWinSelected = binding.Combination.Modifiers.HasFlag(ModifierKeys.Windows);
		SelectedKey = binding.Combination.Key;
		Label = binding.Label;
		Target = binding.Target;
	}

	private HotKeyCombination BuildCombination()
	{
		var modifiers = ModifierKeys.None;
		if (IsCtrlSelected)
		{
			modifiers |= ModifierKeys.Control;
		}
		if (IsAltSelected)
		{
			modifiers |= ModifierKeys.Alt;
		}
		if (IsShiftSelected)
		{
			modifiers |= ModifierKeys.Shift;
		}
		if (IsWinSelected)
		{
			modifiers |= ModifierKeys.Windows;
		}

		return new HotKeyCombination(modifiers, SelectedKey);
	}

	/// <summary>
	/// キー選択欄の候補として、アルファベット(A-Z)・数字(0-9)・ファンクションキー(F1-F12)を列挙する。
	/// </summary>
	private static IReadOnlyList<Key> BuildAvailableKeys()
	{
		var keys = new List<Key>();
		for (var key = Key.A; key <= Key.Z; key++)
		{
			keys.Add(key);
		}
		for (var key = Key.D0; key <= Key.D9; key++)
		{
			keys.Add(key);
		}
		for (var key = Key.F1; key <= Key.F12; key++)
		{
			keys.Add(key);
		}
		return keys;
	}
}
