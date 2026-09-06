using System.Windows.Input;
using GlobalHotkeyLauncher.ViewModels;

namespace GlobalHotkeyLauncher.Tests;

/// <summary>
/// <see cref="MainViewModel"/> のテスト。実際のOSホットキー登録・プロセス起動は行わず、
/// <see cref="FakeHotKeyRegistrar"/>/<see cref="FakeCommandLauncher"/>を使う。
/// </summary>
public class MainViewModelTests
{
	private static MainViewModel CreateViewModel(FakeHotKeyRegistrar? registrar = null, FakeCommandLauncher? launcher = null) =>
		new(registrar ?? new FakeHotKeyRegistrar(), launcher ?? new FakeCommandLauncher());

	private static void SetValidInput(MainViewModel viewModel, Key key = Key.L)
	{
		viewModel.IsCtrlSelected = true;
		viewModel.SelectedKey = key;
		viewModel.Label = "メモ帳を開く";
		viewModel.Target = "notepad.exe";
	}

	/// <summary>
	/// パス条件: 有効な組み合わせで登録すると、Registrarへ登録されBindingsに追加されること
	/// </summary>
	[Fact]
	public void AddHotKeyCommand_有効な組み合わせで登録するとBindingsに追加される()
	{
		var registrar = new FakeHotKeyRegistrar();
		var viewModel = CreateViewModel(registrar);
		SetValidInput(viewModel);

		viewModel.AddHotKeyCommand.Execute(null);

		var binding = Assert.Single(viewModel.Bindings);
		Assert.Equal("Ctrl+L", binding.Combination.ToDisplayString());
		Assert.Equal("メモ帳を開く", binding.Label);
		Assert.Equal("notepad.exe", binding.Target);
		Assert.Single(registrar.RegisteredIds);
	}

	/// <summary>
	/// パス条件: 登録に成功すると入力欄(説明・対象)がクリアされること
	/// </summary>
	[Fact]
	public void AddHotKeyCommand_登録に成功すると入力欄がクリアされる()
	{
		var viewModel = CreateViewModel();
		SetValidInput(viewModel);

		viewModel.AddHotKeyCommand.Execute(null);

		Assert.Equal(string.Empty, viewModel.Label);
		Assert.Equal(string.Empty, viewModel.Target);
	}

	/// <summary>
	/// パス条件: 既に登録済みと同じ組み合わせを登録しようとすると、Registrarを呼ばずエラーになること
	/// </summary>
	[Fact]
	public void AddHotKeyCommand_重複する組み合わせは登録されない()
	{
		var registrar = new FakeHotKeyRegistrar();
		var viewModel = CreateViewModel(registrar);
		SetValidInput(viewModel);
		viewModel.AddHotKeyCommand.Execute(null);

		SetValidInput(viewModel);
		viewModel.Label = "別のアプリ";
		viewModel.AddHotKeyCommand.Execute(null);

		Assert.Single(viewModel.Bindings);
		Assert.Single(registrar.RegisteredIds);
		Assert.False(string.IsNullOrEmpty(viewModel.ErrorMessage));
	}

	/// <summary>
	/// パス条件: Registrarが登録失敗(他アプリ使用中等)を返した場合、Bindingsに追加されずエラーになること
	/// </summary>
	[Fact]
	public void AddHotKeyCommand_Registrarが登録失敗を返すとBindingsに追加されない()
	{
		var registrar = new FakeHotKeyRegistrar { NextRegisterResult = false };
		var viewModel = CreateViewModel(registrar);
		SetValidInput(viewModel);

		viewModel.AddHotKeyCommand.Execute(null);

		Assert.Empty(viewModel.Bindings);
		Assert.False(string.IsNullOrEmpty(viewModel.ErrorMessage));
	}

	/// <summary>
	/// パス条件: キー未選択のまま登録しようとすると、Registrarを呼ばずエラーになること
	/// </summary>
	[Fact]
	public void AddHotKeyCommand_無効な組み合わせはRegistrarを呼ばずエラーになる()
	{
		var registrar = new FakeHotKeyRegistrar();
		var viewModel = CreateViewModel(registrar);
		viewModel.IsCtrlSelected = true;
		viewModel.SelectedKey = Key.None;
		viewModel.Label = "メモ帳を開く";
		viewModel.Target = "notepad.exe";

		viewModel.AddHotKeyCommand.Execute(null);

		Assert.Empty(viewModel.Bindings);
		Assert.Empty(registrar.RegisteredIds);
		Assert.False(string.IsNullOrEmpty(viewModel.ErrorMessage));
	}

	/// <summary>
	/// パス条件: RemoveHotKeyCommandを実行すると、RegistrarのUnregisterが呼ばれBindingsから削除されること
	/// </summary>
	[Fact]
	public void RemoveHotKeyCommand_実行するとUnregisterされBindingsから削除される()
	{
		var registrar = new FakeHotKeyRegistrar();
		var viewModel = CreateViewModel(registrar);
		SetValidInput(viewModel);
		viewModel.AddHotKeyCommand.Execute(null);
		var binding = viewModel.Bindings[0];

		viewModel.RemoveHotKeyCommand.Execute(binding);

		Assert.Empty(viewModel.Bindings);
		Assert.Equal([binding.Id], registrar.UnregisteredIds);
	}

	/// <summary>
	/// パス条件: EditHotKeyCommandを実行すると、Unregisterされ一覧から削除され、入力欄に元の値が復元されること
	/// </summary>
	[Fact]
	public void EditHotKeyCommand_実行するとUnregisterされ入力欄に値が復元される()
	{
		var registrar = new FakeHotKeyRegistrar();
		var viewModel = CreateViewModel(registrar);
		SetValidInput(viewModel);
		viewModel.AddHotKeyCommand.Execute(null);
		var binding = viewModel.Bindings[0];

		viewModel.EditHotKeyCommand.Execute(binding);

		Assert.Empty(viewModel.Bindings);
		Assert.Equal([binding.Id], registrar.UnregisteredIds);
		Assert.True(viewModel.IsCtrlSelected);
		Assert.Equal(Key.L, viewModel.SelectedKey);
		Assert.Equal("メモ帳を開く", viewModel.Label);
		Assert.Equal("notepad.exe", viewModel.Target);
	}

	/// <summary>
	/// パス条件: 登録済みIDでホットキー発火を処理すると、対応するLauncherが正しい対象で呼ばれ
	/// ExecutionLogに記録されること
	/// </summary>
	[Fact]
	public void HandleHotKeyTriggered_登録済みIDの場合Launcherが呼ばれログに記録される()
	{
		var launcher = new FakeCommandLauncher();
		var viewModel = CreateViewModel(launcher: launcher);
		SetValidInput(viewModel);
		viewModel.AddHotKeyCommand.Execute(null);
		var binding = viewModel.Bindings[0];

		viewModel.HandleHotKeyTriggered(binding.Id);

		Assert.Equal(["notepad.exe"], launcher.LaunchedTargets);
		Assert.Contains(viewModel.ExecutionLog, entry => entry.Contains("メモ帳を開く"));
	}

	/// <summary>
	/// パス条件: 登録済みIDでホットキー発火を処理した際にLauncherがfalse(起動失敗)を返すと、
	/// ExecutionLogに失敗を示すメッセージが記録されること。
	/// </summary>
	[Fact]
	public void HandleHotKeyTriggered_Launcherがfalseを返すと実行失敗がログに記録される()
	{
		var launcher = new FakeCommandLauncher { NextLaunchResult = false };
		var viewModel = CreateViewModel(launcher: launcher);
		SetValidInput(viewModel);
		viewModel.AddHotKeyCommand.Execute(null);
		var binding = viewModel.Bindings[0];

		viewModel.HandleHotKeyTriggered(binding.Id);

		Assert.Contains(viewModel.ExecutionLog, entry => entry.Contains("失敗") && entry.Contains("メモ帳を開く"));
	}

	/// <summary>
	/// パス条件: 未登録のIDでホットキー発火を処理しても、例外にならずLauncherも呼ばれないこと
	/// </summary>
	[Fact]
	public void HandleHotKeyTriggered_未登録IDの場合何も起きない()
	{
		var launcher = new FakeCommandLauncher();
		var viewModel = CreateViewModel(launcher: launcher);

		viewModel.HandleHotKeyTriggered(999);

		Assert.Empty(launcher.LaunchedTargets);
	}

	/// <summary>
	/// パス条件: 2件登録すると、それぞれ異なるIDがRegistrarに渡されること
	/// </summary>
	[Fact]
	public void AddHotKeyCommand_2件登録すると異なるIDがRegistrarに渡される()
	{
		var registrar = new FakeHotKeyRegistrar();
		var viewModel = CreateViewModel(registrar);
		SetValidInput(viewModel, Key.L);
		viewModel.AddHotKeyCommand.Execute(null);
		SetValidInput(viewModel, Key.M);
		viewModel.Label = "別のアプリ";
		viewModel.Target = "calc.exe";

		viewModel.AddHotKeyCommand.Execute(null);

		Assert.Equal(2, registrar.RegisteredIds.Count);
		Assert.NotEqual(registrar.RegisteredIds[0], registrar.RegisteredIds[1]);
	}
}
