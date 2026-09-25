namespace WpfLearningLab.Shared.Mvvm.Tests;

/// <summary>
/// <see cref="RelayCommand"/> の単体テスト。
/// </summary>
public class RelayCommandTests
{
	/// <summary>
	/// パス条件: Executeを呼ぶとデリゲートが実行されること
	/// </summary>
	[Fact]
	public void Execute_呼ぶとデリゲートが実行される()
	{
		var executed = false;
		var command = new RelayCommand(() => executed = true);

		command.Execute(null);

		Assert.True(executed);
	}

	/// <summary>
	/// パス条件: CanExecuteがfalseを返す条件でCanExecuteを呼ぶとfalseが返ること
	/// </summary>
	[Fact]
	public void CanExecute_条件を満たさない場合falseを返す()
	{
		var command = new RelayCommand(() => { }, () => false);

		var result = command.CanExecute(null);

		Assert.False(result);
	}

	/// <summary>
	/// パス条件: RaiseCanExecuteChangedを呼ぶとCanExecuteChangedイベントが発火すること
	/// </summary>
	[Fact]
	public void RaiseCanExecuteChanged_呼び出すとCanExecuteChangedが発火する()
	{
		var command = new RelayCommand(() => { });
		var raised = false;
		command.CanExecuteChanged += (_, _) => raised = true;

		command.RaiseCanExecuteChanged();

		Assert.True(raised);
	}
}
