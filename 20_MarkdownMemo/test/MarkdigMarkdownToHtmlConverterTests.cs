using MarkdownMemo.Services;

namespace MarkdownMemo.Tests;

/// <summary>
/// <see cref="MarkdigMarkdownToHtmlConverter"/> の単体テスト。
/// </summary>
public class MarkdigMarkdownToHtmlConverterTests
{
	/// <summary>
	/// パス条件: 見出し記法(#)がh1タグに変換されること
	/// </summary>
	[Fact]
	public void Convert_見出しがh1タグに変換される()
	{
		var converter = new MarkdigMarkdownToHtmlConverter();

		var html = converter.Convert("# タイトル");

		Assert.StartsWith("<h1", html);
		Assert.Contains(">タイトル</h1>", html);
	}

	/// <summary>
	/// パス条件: 太字記法(**)がstrongタグに変換されること
	/// </summary>
	[Fact]
	public void Convert_太字がstrongタグに変換される()
	{
		var converter = new MarkdigMarkdownToHtmlConverter();

		var html = converter.Convert("**重要**");

		Assert.Contains("<strong>重要</strong>", html);
	}

	/// <summary>
	/// パス条件: 箇条書き記法(-)がul/liタグに変換されること
	/// </summary>
	[Fact]
	public void Convert_箇条書きがulリストに変換される()
	{
		var converter = new MarkdigMarkdownToHtmlConverter();

		var html = converter.Convert("- 項目1\n- 項目2");

		Assert.Contains("<ul>", html);
		Assert.Contains("<li>項目1</li>", html);
		Assert.Contains("<li>項目2</li>", html);
	}

	/// <summary>
	/// パス条件: コードブロック記法(```)がpre/codeタグに変換されること
	/// </summary>
	[Fact]
	public void Convert_コードブロックがpreCodeタグに変換される()
	{
		var converter = new MarkdigMarkdownToHtmlConverter();

		var html = converter.Convert("```\nvar x = 1;\n```");

		Assert.Contains("<pre>", html);
		Assert.Contains("<code>", html);
		Assert.Contains("var x = 1;", html);
	}
}
