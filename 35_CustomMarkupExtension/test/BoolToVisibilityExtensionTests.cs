using CustomMarkupExtension.Converters;
using CustomMarkupExtension.MarkupExtensions;

namespace CustomMarkupExtension.Tests;

/// <summary>
/// <see cref="BoolToVisibilityExtension"/>のテスト。
/// 実際のXAML解析(<see cref="IServiceProvider"/>)を経由せず、Binding構築ロジック(<see cref="BoolToVisibilityExtension.BuildBinding"/>)を直接検証する。
/// </summary>
public class BoolToVisibilityExtensionTests
{
	/// <summary>
	/// パス条件: 指定したPathでBindingが構築されること。
	/// </summary>
	[Fact]
	public void BuildBinding_指定したPathでBindingが構築される()
	{
		var extension = new BoolToVisibilityExtension { Path = "IsChecked" };

		var binding = extension.BuildBinding();

		Assert.Equal("IsChecked", binding.Path.Path);
	}

	/// <summary>
	/// パス条件: BindingのConverterにInvertibleBoolToVisibilityConverterが設定されること。
	/// </summary>
	[Fact]
	public void BuildBinding_ConverterにInvertibleBoolToVisibilityConverterが設定される()
	{
		var extension = new BoolToVisibilityExtension { Path = "X" };

		var binding = extension.BuildBinding();

		Assert.IsType<InvertibleBoolToVisibilityConverter>(binding.Converter);
	}

	/// <summary>
	/// パス条件: InvertプロパティがBindingのConverterParameterに反映されること。
	/// </summary>
	[Fact]
	public void BuildBinding_InvertがConverterParameterに反映される()
	{
		var extension = new BoolToVisibilityExtension { Path = "X", Invert = true };

		var binding = extension.BuildBinding();

		Assert.Equal(true, binding.ConverterParameter);
	}

	/// <summary>
	/// パス条件: ElementNameを指定した場合、BindingのElementNameに反映されること。
	/// </summary>
	[Fact]
	public void BuildBinding_ElementNameがBindingに反映される()
	{
		var extension = new BoolToVisibilityExtension { Path = "IsChecked", ElementName = "MyCheckBox" };

		var binding = extension.BuildBinding();

		Assert.Equal("MyCheckBox", binding.ElementName);
	}

	/// <summary>
	/// パス条件: BuildBindingを複数回呼んでも、Converterはステートレスなため
	/// 同一インスタンスが使い回されること(呼ぶたびに新規生成しない)。
	/// </summary>
	[Fact]
	public void BuildBinding_複数回呼んでもConverterは同一インスタンスが使い回される()
	{
		var extension = new BoolToVisibilityExtension { Path = "X" };

		var first = extension.BuildBinding();
		var second = extension.BuildBinding();

		Assert.Same(first.Converter, second.Converter);
	}
}
