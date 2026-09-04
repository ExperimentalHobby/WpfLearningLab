using System.Windows.Markup;

namespace CustomMarkupExtension.MarkupExtensions;

/// <summary>
/// 単位付きの値を別の単位に変換した数値を返すMarkupExtension。
/// XAML上で <c>Width="{local:UnitConversion Value=5, From=Centimeter, To=Pixel}"</c> のように使う。
/// </summary>
public class UnitConversionExtension : MarkupExtension
{
	/// <summary>変換元の値。</summary>
	public double Value { get; set; }

	/// <summary>変換元の単位。</summary>
	public UnitOfLength From { get; set; } = UnitOfLength.Pixel;

	/// <summary>変換先の単位。</summary>
	public UnitOfLength To { get; set; } = UnitOfLength.Pixel;

	/// <inheritdoc/>
	public override object ProvideValue(IServiceProvider serviceProvider)
		=> UnitConverter.Convert(Value, From, To);
}
