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
	/// <remarks>
	/// 配列全体を一括でデシリアライズすると、1件でも必須の<see cref="StickyNoteData.Id"/>を
	/// 欠く要素があった場合に配列全体が例外となり、正常な他の付箋まで巻き込んで全消失して
	/// しまう。これを避けるため、配列の要素を1件ずつ個別にデシリアライズし、壊れた要素だけを
	/// スキップして残りの要素は復元できるようにする。
	/// </remarks>
	/// <param name="json">デシリアライズするJSON文字列。</param>
	/// <returns>復元した付箋データのリスト。壊れた要素はスキップされる。</returns>
	public IReadOnlyList<StickyNoteData> Deserialize(string json)
	{
		if (string.IsNullOrWhiteSpace(json))
		{
			return [];
		}

		try
		{
			using var document = JsonDocument.Parse(json);
			if (document.RootElement.ValueKind != JsonValueKind.Array)
			{
				return [];
			}

			var notes = new List<StickyNoteData>();
			foreach (var element in document.RootElement.EnumerateArray())
			{
				try
				{
					var note = element.Deserialize<StickyNoteData>(Options);
					if (note is not null)
					{
						notes.Add(note);
					}
				}
				catch (JsonException)
				{
					// この要素だけを読み飛ばし、他の正常な付箋の復元を続ける。
				}
			}

			return notes;
		}
		catch (JsonException)
		{
			return [];
		}
	}
}
