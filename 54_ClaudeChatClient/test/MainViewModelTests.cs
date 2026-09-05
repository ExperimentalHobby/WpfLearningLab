using ClaudeChatClient.Services;
using ClaudeChatClient.Tests.Fakes;
using ClaudeChatClient.ViewModels;

namespace ClaudeChatClient.Tests;

public class MainViewModelTests
{
	private static (MainViewModel vm, FakeApiKeyStore store, FakeClaudeApiClient apiClient) CreateViewModel(
		FakeApiKeyStore? store = null)
	{
		store ??= new FakeApiKeyStore();
		var apiClient = new FakeClaudeApiClient();
		var vm = new MainViewModel(store, new AesApiKeyCryptoService(), _ => apiClient);
		return (vm, store, apiClient);
	}

	/// <summary>
	/// パス条件: APIキー未保存の状態ではIsFirstRun=trueであること。
	/// </summary>
	[Fact]
	public void Constructor_NoStoredKey_IsFirstRunTrue()
	{
		var (vm, _, _) = CreateViewModel();

		Assert.True(vm.IsFirstRun);
		Assert.False(vm.IsUnlocked);
	}

	/// <summary>
	/// パス条件: 初回セットアップでマスターパスワードとAPIキーを入力しSetupCommandを実行すると、
	/// 保存されIsUnlocked=trueになること。
	/// </summary>
	[Fact]
	public void SetupCommand_ValidInput_SavesAndUnlocks()
	{
		var (vm, store, _) = CreateViewModel();
		vm.MasterPasswordInput = "master-password";
		vm.ApiKeyInput = "sk-ant-test-key";

		vm.SetupCommand.Execute(null);

		Assert.True(vm.IsUnlocked);
		Assert.True(store.TryLoad(out _));
	}

	/// <summary>
	/// パス条件: 既存環境で正しいマスターパスワードを入力しUnlockCommandを実行すると、IsUnlocked=trueになること。
	/// </summary>
	[Fact]
	public void UnlockCommand_CorrectPassword_Unlocks()
	{
		var store = new FakeApiKeyStore();
		var (setupVm, _, _) = CreateViewModel(store);
		setupVm.MasterPasswordInput = "master-password";
		setupVm.ApiKeyInput = "sk-ant-test-key";
		setupVm.SetupCommand.Execute(null);

		var (vm, _, _) = CreateViewModel(store);
		vm.MasterPasswordInput = "master-password";

		vm.UnlockCommand.Execute(null);

		Assert.True(vm.IsUnlocked);
		Assert.Empty(vm.ErrorMessage);
	}

	/// <summary>
	/// パス条件: 既存環境で誤ったマスターパスワードを入力すると、ロックのままエラーメッセージが表示されること。
	/// </summary>
	[Fact]
	public void UnlockCommand_WrongPassword_StaysLockedWithError()
	{
		var store = new FakeApiKeyStore();
		var (setupVm, _, _) = CreateViewModel(store);
		setupVm.MasterPasswordInput = "master-password";
		setupVm.ApiKeyInput = "sk-ant-test-key";
		setupVm.SetupCommand.Execute(null);

		var (vm, _, _) = CreateViewModel(store);
		vm.MasterPasswordInput = "wrong-password";

		vm.UnlockCommand.Execute(null);

		Assert.False(vm.IsUnlocked);
		Assert.NotEmpty(vm.ErrorMessage);
	}

	/// <summary>
	/// パス条件: 送信するとUserメッセージが履歴に追加され、ストリーミング中のテキストが
	/// Assistantメッセージとして逐次更新されること。
	/// </summary>
	[Fact]
	public async Task SendAsync_StreamsResponse_UpdatesAssistantMessageIncrementally()
	{
		var (vm, _, apiClient) = CreateViewModel();
		vm.MasterPasswordInput = "master-password";
		vm.ApiKeyInput = "sk-ant-test-key";
		vm.SetupCommand.Execute(null);
		apiClient.ChunksToYield = ["こんにちは", "、世界"];
		vm.InputText = "こんにちは";

		await vm.SendAsync();

		Assert.Equal(2, vm.Messages.Count);
		Assert.Equal(Models.ChatRole.User, vm.Messages[0].Role);
		Assert.Equal("こんにちは", vm.Messages[0].Content);
		Assert.Equal(Models.ChatRole.Assistant, vm.Messages[1].Role);
		Assert.Equal("こんにちは、世界", vm.Messages[1].Content);
	}

	/// <summary>
	/// パス条件: 送信中にCancelCommandを実行すると、進行中のリクエストのCancellationTokenが
	/// キャンセルされ、それ以降のテキスト更新が止まること。
	/// </summary>
	[Fact]
	public async Task CancelCommand_DuringSend_CancelsInFlightRequest()
	{
		var (vm, _, apiClient) = CreateViewModel();
		vm.MasterPasswordInput = "master-password";
		vm.ApiKeyInput = "sk-ant-test-key";
		vm.SetupCommand.Execute(null);
		apiClient.ChunksToYield = ["A", "B", "C"];
		apiClient.YieldBeforeEachChunk = true;
		vm.InputText = "test";

		var sendTask = vm.SendAsync();
		vm.CancelCommand.Execute(null);
		await sendTask;

		Assert.True(apiClient.LastCancellationToken.IsCancellationRequested);
		Assert.DoesNotContain("C", vm.Messages[1].Content);
	}
}
