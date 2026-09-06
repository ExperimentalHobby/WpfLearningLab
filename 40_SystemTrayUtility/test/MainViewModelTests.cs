using SystemTrayUtility.Services;
using SystemTrayUtility.Tests.Fakes;
using SystemTrayUtility.ViewModels;

namespace SystemTrayUtility.Tests;

/// <summary>
/// <see cref="MainViewModel"/>のテスト。実レジストリに触れない<see cref="InMemoryStartupRegistrar"/>を使う。
/// </summary>
public class MainViewModelTests
{
	/// <summary>
	/// パス条件: 起動時、IsStartupEnabledがレジストラの現在の登録状態を反映すること。
	/// </summary>
	[Fact]
	public void Constructor_IsStartupEnabledがレジストラの登録状態を反映する()
	{
		var registrar = new InMemoryStartupRegistrar();
		registrar.Register();

		var viewModel = new MainViewModel(registrar);

		Assert.True(viewModel.IsStartupEnabled);
	}

	/// <summary>
	/// パス条件: IsStartupEnabledをtrueに設定すると、レジストラのRegisterが呼ばれること。
	/// </summary>
	[Fact]
	public void IsStartupEnabled_trueに設定するとRegisterされる()
	{
		var registrar = new InMemoryStartupRegistrar();
		var viewModel = new MainViewModel(registrar);

		viewModel.IsStartupEnabled = true;

		Assert.True(registrar.IsRegistered());
	}

	/// <summary>
	/// パス条件: IsStartupEnabledをfalseに設定すると、レジストラのUnregisterが呼ばれること。
	/// </summary>
	[Fact]
	public void IsStartupEnabled_falseに設定するとUnregisterされる()
	{
		var registrar = new InMemoryStartupRegistrar();
		registrar.Register();
		var viewModel = new MainViewModel(registrar) { IsStartupEnabled = false };

		Assert.False(registrar.IsRegistered());
	}

	/// <summary>
	/// パス条件: TestNotifyCommandを実行すると、TestNotifyRequestedイベントが発火すること。
	/// </summary>
	[Fact]
	public void TestNotifyCommand_実行するとTestNotifyRequestedイベントが発火する()
	{
		var viewModel = new MainViewModel(new InMemoryStartupRegistrar());
		var raised = false;
		viewModel.TestNotifyRequested += () => raised = true;

		viewModel.TestNotifyCommand.Execute(null);

		Assert.True(raised);
	}

	/// <summary>
	/// パス条件: IsStartupEnabledをtrueに設定した際にレジストラのRegisterが例外をスローすると、
	/// クラッシュせずIsStartupEnabledは変更前(false)のままで、StatusTextにエラーメッセージが設定されること。
	/// </summary>
	[Fact]
	public void IsStartupEnabled_Registerが例外をスローすると状態は変わらずStatusTextにエラーが設定される()
	{
		var viewModel = new MainViewModel(new ThrowingStartupRegistrar());

		viewModel.IsStartupEnabled = true;

		Assert.False(viewModel.IsStartupEnabled);
		Assert.Contains("失敗しました", viewModel.StatusText);
	}

	/// <summary>
	/// パス条件: コンストラクタでレジストラのIsRegisteredが例外をスローすると、
	/// クラッシュせずIsStartupEnabledはfalseで初期化され、StatusTextにエラーメッセージが設定されること。
	/// </summary>
	[Fact]
	public void Constructor_IsRegisteredが例外をスローするとIsStartupEnabledはfalseで初期化されStatusTextにエラーが設定される()
	{
		var viewModel = new MainViewModel(new ThrowingStartupRegistrar());

		Assert.False(viewModel.IsStartupEnabled);
		Assert.Contains("失敗しました", viewModel.StatusText);
	}
}
