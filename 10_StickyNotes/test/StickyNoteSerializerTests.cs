namespace StickyNotes.Tests;

/// <summary>
/// <see cref="StickyNoteSerializer"/> のシリアライズ/デシリアライズに関するテスト。
/// </summary>
public class StickyNoteSerializerTests
{
	/// <summary>
	/// パス条件: 空リストをSerializeすると空配列のJSON文字列を返すこと。
	/// </summary>
	[Fact]
	public void Serialize_EmptyList_ReturnsEmptyJsonArray()
	{
		var serializer = new StickyNoteSerializer();

		var json = serializer.Serialize([]);

		Assert.Equal("[]", json);
	}

	/// <summary>
	/// パス条件: 1件のデータをSerializeすると、Id・Textを含むJSON文字列を返すこと。
	/// </summary>
	[Fact]
	public void Serialize_SingleNote_ContainsExpectedFields()
	{
		var serializer = new StickyNoteSerializer();
		var note = new StickyNoteData { Id = "note-1", Text = "買い物リスト" };

		var json = serializer.Serialize([note]);

		Assert.Contains("\"Id\":\"note-1\"", json);
		Assert.Contains("\"Text\":\"買い物リスト\"", json);
	}

	/// <summary>
	/// パス条件: 正常なJSON文字列をDeserializeすると、正しいデータリストを復元できること。
	/// </summary>
	[Fact]
	public void Deserialize_ValidJson_ReturnsCorrectNotes()
	{
		var serializer = new StickyNoteSerializer();
		const string json = """[{"Id":"note-1","Text":"買い物リスト","Left":10,"Top":20,"Width":200,"Height":180,"ColorHex":"#FFF9C4"}]""";

		var result = serializer.Deserialize(json);

		var note = Assert.Single(result);
		Assert.Equal("note-1", note.Id);
		Assert.Equal("買い物リスト", note.Text);
		Assert.Equal(10, note.Left);
		Assert.Equal(20, note.Top);
	}

	/// <summary>
	/// パス条件: 空文字をDeserializeすると空リストを返すこと。
	/// </summary>
	[Fact]
	public void Deserialize_EmptyString_ReturnsEmptyList()
	{
		var serializer = new StickyNoteSerializer();

		var result = serializer.Deserialize(string.Empty);

		Assert.Empty(result);
	}

	/// <summary>
	/// パス条件: 不正なJSON文字列をDeserializeしても例外を投げず空リストを返すこと。
	/// </summary>
	[Fact]
	public void Deserialize_InvalidJson_ReturnsEmptyListWithoutThrowing()
	{
		var serializer = new StickyNoteSerializer();

		var result = serializer.Deserialize("{this is not valid json");

		Assert.Empty(result);
	}

	/// <summary>
	/// パス条件: 複数件のJSONをDeserializeすると、元の順序が保持されること。
	/// </summary>
	[Fact]
	public void Deserialize_MultipleNotes_PreservesOrder()
	{
		var serializer = new StickyNoteSerializer();
		const string json = """
			[
				{"Id":"note-1","Text":"1件目"},
				{"Id":"note-2","Text":"2件目"},
				{"Id":"note-3","Text":"3件目"}
			]
			""";

		var result = serializer.Deserialize(json);

		Assert.Equal(["note-1", "note-2", "note-3"], result.Select(n => n.Id));
	}

	/// <summary>
	/// パス条件: SerializeしてからDeserializeすると、元のデータと一致すること(往復変換)。
	/// </summary>
	[Fact]
	public void RoundTrip_SerializeThenDeserialize_ReturnsEquivalentData()
	{
		var serializer = new StickyNoteSerializer();
		var original = new StickyNoteData
		{
			Id = "note-1",
			Text = "TODO: 買い物",
			Left = 100,
			Top = 50,
			Width = 240,
			Height = 200,
			ColorHex = "#B3E5FC",
		};

		var json = serializer.Serialize([original]);
		var result = serializer.Deserialize(json);

		var restored = Assert.Single(result);
		Assert.Equal(original.Id, restored.Id);
		Assert.Equal(original.Text, restored.Text);
		Assert.Equal(original.Left, restored.Left);
		Assert.Equal(original.Top, restored.Top);
		Assert.Equal(original.Width, restored.Width);
		Assert.Equal(original.Height, restored.Height);
		Assert.Equal(original.ColorHex, restored.ColorHex);
	}

	/// <summary>
	/// パス条件: StickyNoteDataをIdのみ指定して生成すると、Width/Height/ColorHexに既定値が入ること。
	/// </summary>
	[Fact]
	public void StickyNoteData_DefaultValues_AreAsExpected()
	{
		var note = new StickyNoteData { Id = "note-1" };

		Assert.Equal(220, note.Width);
		Assert.Equal(220, note.Height);
		Assert.Equal("#FFF9C4", note.ColorHex);
		Assert.Equal(string.Empty, note.Text);
	}

	/// <summary>
	/// パス条件: 複数件のうち1件だけ必須のIdを欠く壊れた要素があっても、その要素だけが
	/// スキップされ、他の正常な要素は復元できること(1件の破損で全付箋が消失しないこと)。
	/// </summary>
	[Fact]
	public void Deserialize_OneEntryMissingRequiredId_SkipsOnlyThatEntry()
	{
		var serializer = new StickyNoteSerializer();
		const string json = """
			[
				{"Id":"note-1","Text":"1件目"},
				{"Text":"Idを欠く壊れた要素"},
				{"Id":"note-3","Text":"3件目"}
			]
			""";

		var result = serializer.Deserialize(json);

		Assert.Equal(["note-1", "note-3"], result.Select(n => n.Id));
	}
}
