using CustomMarkupExtension.MarkupExtensions;
using CustomMarkupExtension.Models;

namespace CustomMarkupExtension.Tests;

/// <summary>
/// <see cref="EnumValuesExtension"/>のテスト。
/// </summary>
public class EnumValuesExtensionTests
{
	/// <summary>
	/// パス条件: 指定した列挙型の全値を返すこと。
	/// </summary>
	[Fact]
	public void ProvideValue_指定した列挙型の全値を返す()
	{
		var extension = new EnumValuesExtension(typeof(Priority));

		var result = (Priority[])extension.ProvideValue(null!);

		Assert.Equal([Priority.Low, Priority.Medium, Priority.High], result);
	}

	/// <summary>
	/// パス条件: 列挙型でない型を指定した場合、ArgumentExceptionをスローすること。
	/// </summary>
	[Fact]
	public void ProvideValue_列挙型でない場合ArgumentExceptionをスローする()
	{
		var extension = new EnumValuesExtension(typeof(string));

		Assert.Throws<ArgumentException>(() => extension.ProvideValue(null!));
	}
}
