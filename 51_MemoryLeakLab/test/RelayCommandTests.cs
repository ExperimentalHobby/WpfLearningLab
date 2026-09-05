using MemoryLeakLab.ViewModels;

namespace MemoryLeakLab.Tests;

public class RelayCommandTests
{
	/// <summary>
	/// パス条件: Executeを呼ぶとコンストラクタで渡したデリゲートが実行されること。
	/// </summary>
	[Fact]
	public void Execute_InvokesDelegate()
	{
		var invoked = false;
		var command = new RelayCommand(() => invoked = true);

		command.Execute(null);

		Assert.True(invoked);
	}

	/// <summary>
	/// パス条件: canExecuteがfalseを返す場合、CanExecuteがfalseになること。
	/// </summary>
	[Fact]
	public void CanExecute_ReflectsCanExecuteDelegate()
	{
		var command = new RelayCommand(() => { }, () => false);

		Assert.False(command.CanExecute(null));
	}
}
