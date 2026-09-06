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
	/// パス条件: 列挙型でない型を指定した場合、コンストラクタ呼び出し時点でArgumentExceptionを
	/// スローすること(XAML解析時ではなく、より早いタイミングで検知できるようにする)。
	/// </summary>
	[Fact]
	public void Constructor_列挙型でない場合ArgumentExceptionをスローする()
	{
		Assert.Throws<ArgumentException>(() => new EnumValuesExtension(typeof(string)));
	}
}
