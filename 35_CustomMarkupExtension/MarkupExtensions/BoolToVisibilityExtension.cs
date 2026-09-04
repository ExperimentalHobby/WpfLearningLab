using System.Windows.Data;
using System.Windows.Markup;
using CustomMarkupExtension.Converters;

namespace CustomMarkupExtension.MarkupExtensions;

/// <summary>
/// 「boolプロパティにバインドしてVisibilityへ変換する」という頻出パターンを1つのMarkupExtensionにまとめたもの。
/// XAML上で <c>Visibility="{local:BoolToVisibility Path=IsChecked}"</c> のように使い、
/// <c>Binding + BooleanToVisibilityConverter</c>を毎回書く手間を省く。
/// 内部では実際に<see cref="Binding"/>を組み立て、<see cref="ProvideValue"/>はそれに委譲する
/// (「バインディングと組み合わせたユースケース」の例)。
/// </summary>
public class BoolToVisibilityExtension : MarkupExtension
{
	/// <summary>バインド元のパス。</summary>
	public string Path { get; set; } = ".";

	/// <summary>結果を反転するかどうか。</summary>
	public bool Invert { get; set; }

	/// <summary>バインド元の要素名(<see cref="Binding.ElementName"/>)。未指定の場合はDataContextから解決する。</summary>
	public string? ElementName { get; set; }

	/// <summary>
	/// このMarkupExtensionが内部で使う<see cref="Binding"/>を組み立てる。
	/// <see cref="ProvideValue"/>から分離することで、実際のXAML解析(<see cref="IServiceProvider"/>)無しでも
	/// Binding構築ロジック自体を単体テストできるようにしている。
	/// </summary>
	public Binding BuildBinding()
	{
		var binding = new Binding(Path)
		{
			Converter = new InvertibleBoolToVisibilityConverter(),
			ConverterParameter = Invert,
		};
		if (ElementName is not null)
		{
			binding.ElementName = ElementName;
		}
		return binding;
	}

	/// <inheritdoc/>
	public override object ProvideValue(IServiceProvider serviceProvider)
		=> BuildBinding().ProvideValue(serviceProvider);
}
