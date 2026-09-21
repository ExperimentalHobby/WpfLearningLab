using MemoryLeakLab.Services;
using MemoryLeakLab.ViewModels;

namespace MemoryLeakLab.Tests;

public class MainViewModelTests
{
	/// <summary>
	/// パス条件: GenerateCommand実行でTotalCountが生成数分増えること。
	/// </summary>
	[Fact]
	public void GenerateCommand_IncreasesTotalCount()
	{
		var vm = new MainViewModel();

		vm.GenerateCommand.Execute(null);

		Assert.Equal(10, vm.TotalCount);
	}

	/// <summary>
	/// パス条件: 生成直後はAliveCountがTotalCountと一致すること。
	/// </summary>
	[Fact]
	public void AliveCount_AfterGenerate_EqualsTotalCount()
	{
		var vm = new MainViewModel();

		vm.GenerateCommand.Execute(null);

		Assert.Equal(vm.TotalCount, vm.AliveCount);
	}

	/// <summary>
	/// パス条件: Bad版モードで生成→参照解放→GC実行しても、AliveCountが0にならないこと(リーク再現)。
	/// </summary>
	[Fact]
	public void BadMode_AliveCount_DoesNotReachZero_AfterReleaseAndGc()
	{
		var vm = new MainViewModel { Mode = LeakMode.Bad };

		vm.GenerateCommand.Execute(null);
		vm.ReleaseReferencesCommand.Execute(null);
		vm.CollectGarbageCommand.Execute(null);

		Assert.Equal(10, vm.AliveCount);
	}

	/// <summary>
	/// パス条件: Good版モードで生成→参照解放→GC実行すると、AliveCountが0になること(修正確認)。
	/// </summary>
	[Fact]
	public void GoodMode_AliveCount_ReachesZero_AfterReleaseAndGc()
	{
		var vm = new MainViewModel { Mode = LeakMode.Good };

		vm.GenerateCommand.Execute(null);
		vm.ReleaseReferencesCommand.Execute(null);
		vm.CollectGarbageCommand.Execute(null);

		Assert.Equal(0, vm.AliveCount);
	}

	/// <summary>
	/// パス条件: GenerateCommand実行時にPublisherへ通知が飛び、既存の生存中購読者のReceivedCountが
	/// 増えること(EventPublisher.SomethingChangedがテスト専用の実質デッドコードでなくなることの確認)。
	/// </summary>
	[Fact]
	public void GenerateCommand_RaisesPublisherEvent_ForExistingSubscriber()
	{
		var publisher = new EventPublisher();
		var probe = new LeakySubscriberViewModel(publisher);
		var vm = new MainViewModel(publisher, new LeakTracker()) { Mode = LeakMode.Bad };

		vm.GenerateCommand.Execute(null);

		Assert.Equal(1, probe.ReceivedCount);
	}
}
