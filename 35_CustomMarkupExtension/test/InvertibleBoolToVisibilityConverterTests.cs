using System.Windows;
using CustomMarkupExtension.Converters;

namespace CustomMarkupExtension.Tests;

/// <summary>
/// <see cref="InvertibleBoolToVisibilityConverter"/>のテスト。
/// </summary>
public class InvertibleBoolToVisibilityConverterTests
{
	/// <summary>
	/// パス条件: trueを渡すとVisibleを返すこと。
	/// </summary>
	[Fact]
	public void Convert_trueの場合Visibleを返す()
	{
		var sut = new InvertibleBoolToVisibilityConverter();
		var result = sut.Convert(true, typeof(Visibility), null, null!);
		Assert.Equal(Visibility.Visible, result);
	}

	/// <summary>
	/// パス条件: falseを渡すとCollapsedを返すこと。
	/// </summary>
	[Fact]
	public void Convert_falseの場合Collapsedを返す()
	{
		var sut = new InvertibleBoolToVisibilityConverter();
		var result = sut.Convert(false, typeof(Visibility), null, null!);
		Assert.Equal(Visibility.Collapsed, result);
	}

	/// <summary>
	/// パス条件: parameterにtrueを渡すと結果が反転すること(true→Collapsed)。
	/// </summary>
	[Fact]
	public void Convert_Invertパラメータを渡すと結果が反転する()
	{
		var sut = new InvertibleBoolToVisibilityConverter();
		var result = sut.Convert(true, typeof(Visibility), true, null!);
		Assert.Equal(Visibility.Collapsed, result);
	}
}
