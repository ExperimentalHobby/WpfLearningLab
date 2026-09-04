using LocalTaskScheduler.ViewModels;

namespace LocalTaskScheduler.Tests;

/// <summary>
/// <see cref="RelayCommand{T}"/> の単体テスト。
/// </summary>
public class RelayCommandOfTTests
{
	/// <summary>
	/// パス条件: Executeを呼ぶと型付きパラメータがそのままデリゲートに渡ること
	/// </summary>
	[Fact]
	public void Execute_型付きパラメータがデリゲートに渡る()
	{
		string? received = null;
		var command = new RelayCommand<string>(value => received = value);

		command.Execute("タスク1");

		Assert.Equal("タスク1", received);
	}

	/// <summary>
	/// パス条件: CanExecuteに渡した型付きパラメータで判定結果が変わること
	/// </summary>
	[Theory]
	[InlineData("", false)]
	[InlineData("タスク1", true)]
	public void CanExecute_型付きパラメータで判定結果が変わる(string name, bool expected)
	{
		var command = new RelayCommand<string>(_ => { }, value => !string.IsNullOrWhiteSpace(value));

		var result = command.CanExecute(name);

		Assert.Equal(expected, result);
	}
}
