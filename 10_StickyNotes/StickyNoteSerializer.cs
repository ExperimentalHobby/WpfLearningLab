using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace StickyNotes;

/// <summary>
/// 付箋データ(<see cref="StickyNoteData"/>)のJSONシリアライズ/デシリアライズを行う。
/// 実際のファイル読み書きは行わず、文字列の変換のみを担当する。
/// </summary>
public class StickyNoteSerializer
{
	private static readonly JsonSerializerOptions Options = new()
	{
		Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
	};

	/// <summary>
	/// 付箋データのリストをJSON文字列に変換する。
	/// </summary>
	/// <param name="notes">シリアライズする付箋データ。</param>
	public string Serialize(IEnumerable<StickyNoteData> notes)
	{
		return JsonSerializer.Serialize(notes.ToList(), Options);
	}

	/// <summary>
	/// JSON文字列を付箋データのリストに変換する。
	/// </summary>
	/// <param name="json">デシリアライズするJSON文字列。</param>
	/// <returns>復元した付箋データのリスト。</returns>
	public IReadOnlyList<StickyNoteData> Deserialize(string json)
	{
		if (string.IsNullOrWhiteSpace(json))
		{
			return [];
		}

		try
		{
			return JsonSerializer.Deserialize<List<StickyNoteData>>(json, Options) ?? [];
		}
		catch (JsonException)
		{
			return [];
		}
	}
}
