namespace MiniDictionary.Tests;

/// <summary>
/// <see cref="DictionaryEngine"/> の単語検索・意味取得に関するテスト。
/// </summary>
public class DictionaryEngineTests
{
	private static DictionaryEngine CreateEngine() => new(new Dictionary<string, string>
	{
		["apple"] = "りんご",
		["application"] = "応用プログラム",
		["banana"] = "バナナ",
	});

	/// <summary>
	/// パス条件: 空クエリでSearchすると、全単語がソートされて返ること。
	/// </summary>
	[Fact]
	public void Search_EmptyQuery_ReturnsAllWordsSorted()
	{
		var engine = CreateEngine();

		var result = engine.Search(string.Empty);

		Assert.Equal(["apple", "application", "banana"], result);
	}

	/// <summary>
	/// パス条件: 前方一致するクエリで、該当する単語だけが返ること。
	/// </summary>
	[Fact]
	public void Search_PrefixMatch_ReturnsMatchingWords()
	{
		var engine = CreateEngine();

		var result = engine.Search("app");

		Assert.Equal(["apple", "application"], result);
	}

	/// <summary>
	/// パス条件: 単語の中間の部分一致でも該当すること。
	/// </summary>
	[Fact]
	public void Search_MiddleSubstring_ReturnsMatchingWords()
	{
		var engine = CreateEngine();

		var result = engine.Search("plic");

		Assert.Equal(["application"], result);
	}

	/// <summary>
	/// パス条件: 大文字小文字を区別せず検索できること。
	/// </summary>
	[Fact]
	public void Search_CaseInsensitive_ReturnsMatchingWords()
	{
		var engine = CreateEngine();

		var result = engine.Search("APP");

		Assert.Equal(["apple", "application"], result);
	}

	/// <summary>
	/// パス条件: 該当する単語がない場合、空リストを返すこと。
	/// </summary>
	[Fact]
	public void Search_NoMatch_ReturnsEmptyList()
	{
		var engine = CreateEngine();

		var result = engine.Search("xyz");

		Assert.Empty(result);
	}

	/// <summary>
	/// パス条件: 検索結果が辞書順にソートされていること。
	/// </summary>
	[Fact]
	public void Search_ResultsAreSorted()
	{
		var engine = CreateEngine();

		var result = engine.Search("a");

		Assert.Equal(["apple", "application", "banana"], result);
	}

	/// <summary>
	/// パス条件: 存在する単語をGetMeaningすると、正しい意味を返すこと。
	/// </summary>
	[Fact]
	public void GetMeaning_ExistingWord_ReturnsMeaning()
	{
		var engine = CreateEngine();

		var meaning = engine.GetMeaning("apple");

		Assert.Equal("りんご", meaning);
	}

	/// <summary>
	/// パス条件: 存在しない単語をGetMeaningするとnullを返すこと。
	/// </summary>
	[Fact]
	public void GetMeaning_NonExistingWord_ReturnsNull()
	{
		var engine = CreateEngine();

		var meaning = engine.GetMeaning("orange");

		Assert.Null(meaning);
	}

	/// <summary>
	/// パス条件: 既定のコンストラクタでインスタンス化すると、サンプルの辞書データが投入されていること。
	/// </summary>
	[Fact]
	public void DefaultConstructor_ContainsSampleEntries()
	{
		var engine = new DictionaryEngine();

		var result = engine.Search(string.Empty);

		Assert.NotEmpty(result);
	}

	/// <summary>
	/// パス条件: nullクエリでSearchするとArgumentNullExceptionが送出されること
	/// (NullReferenceExceptionではなく、引数の問題であることが分かる例外にする)。
	/// </summary>
	[Fact]
	public void Search_NullQuery_ThrowsArgumentNullException()
	{
		var engine = CreateEngine();

		Assert.Throws<ArgumentNullException>(() => engine.Search(null!));
	}
}
