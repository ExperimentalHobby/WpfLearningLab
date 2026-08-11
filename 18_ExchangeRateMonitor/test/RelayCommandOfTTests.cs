using ExchangeRateMonitor.ViewModels;

namespace ExchangeRateMonitor.Tests;

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

		command.Execute("USD/JPY");

		Assert.Equal("USD/JPY", received);
	}

	/// <summary>
	/// パス条件: CanExecuteに渡した型付きパラメータで判定結果が変わること
	/// </summary>
	[Theory]
	[InlineData("", false)]
	[InlineData("USD/JPY", true)]
	public void CanExecute_型付きパラメータで判定結果が変わる(string pair, bool expected)
	{
		var command = new RelayCommand<string>(_ => { }, value => !string.IsNullOrWhiteSpace(value));

		var result = command.CanExecute(pair);

		Assert.Equal(expected, result);
	}
}
