using System.Windows.Markup;

namespace CustomMarkupExtension.MarkupExtensions;

/// <summary>
/// 指定した列挙型の全値を配列で返すMarkupExtension。
/// XAML上で <c>ItemsSource="{local:EnumValues {x:Type local:Priority}}"</c> のように使う。
/// </summary>
public class EnumValuesExtension : MarkupExtension
{
	/// <summary>値一覧を取得する対象の列挙型。</summary>
	public Type EnumType { get; }

	/// <summary>
	/// <see cref="EnumValuesExtension"/>を初期化する。
	/// </summary>
	/// <param name="enumType">値一覧を取得する対象の列挙型。</param>
	public EnumValuesExtension(Type enumType)
	{
		// XAML解析時(ProvideValue呼び出し時)まで検証を遅らせると発見が遅れるため、
		// コンストラクタの時点で検証する。
		if (!enumType.IsEnum)
		{
			throw new ArgumentException($"{enumType}は列挙型ではありません。", nameof(enumType));
		}

		EnumType = enumType;
	}

	/// <inheritdoc/>
	public override object ProvideValue(IServiceProvider serviceProvider) => Enum.GetValues(EnumType);
}
