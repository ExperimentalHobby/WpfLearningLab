using RssReader.ViewModels;

namespace RssReader.Tests;

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

		command.Execute("https://example.com/rss");

		Assert.Equal("https://example.com/rss", received);
	}

	/// <summary>
	/// パス条件: CanExecuteに渡した型付きパラメータで判定結果が変わること
	/// </summary>
	[Theory]
	[InlineData(null, false)]
	[InlineData("https://example.com/rss", true)]
	public void CanExecute_型付きパラメータで判定結果が変わる(string? link, bool expected)
	{
		var command = new RelayCommand<string>(_ => { }, value => !string.IsNullOrEmpty(value));

		var result = command.CanExecute(link);

		Assert.Equal(expected, result);
	}
}
