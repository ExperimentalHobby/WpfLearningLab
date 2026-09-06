using DataWorkbench.Models;

namespace DataWorkbench;

/// <summary>
/// 簡易CSVの読み書きを行うロジック。ダブルクォートで囲まれたフィールド内の
/// カンマ・改行、および <c>""</c> によるクォートのエスケープに対応する。
/// </summary>
public static class CsvParser
{
	/// <summary>
	/// CSVの各行から <see cref="CsvTable"/> を生成する。空行は無視する。
	/// </summary>
	public static CsvTable Parse(IEnumerable<string> lines)
	{
		var table = new CsvTable();
		var isFirstLine = true;
		string? pendingRecord = null;

		void ProcessRecord(string record)
		{
			if (string.IsNullOrEmpty(record))
			{
				return;
			}

			var fields = SplitLine(record);

			if (isFirstLine)
			{
				table.Headers.AddRange(DisambiguateHeaders(fields));
				isFirstLine = false;
				return;
			}

			var row = new CsvRow();
			for (var i = 0; i < table.Headers.Count; i++)
			{
				row[table.Headers[i]] = i < fields.Count ? fields[i] : string.Empty;
			}

			table.Rows.Add(row);
		}

		foreach (var line in lines)
		{
			// ダブルクォートで囲まれたフィールド内に改行が含まれる場合、1つのCSVレコードが
			// 複数の物理行(呼び出し元がFile.ReadLines等で分割した単位)にまたがる。
			// クォートが閉じていない間は次の行と改行で結合し、レコードとして確定させない。
			pendingRecord = pendingRecord is null ? line : pendingRecord + "\n" + line;

			if (EndsInsideQuotedField(pendingRecord))
			{
				continue;
			}

			ProcessRecord(pendingRecord);
			pendingRecord = null;
		}

		// クォートが閉じられないまま入力が終わった場合(不正なCSV)でも、
		// それまでに蓄積した内容を1レコードとして扱う。
		if (pendingRecord is not null)
		{
			ProcessRecord(pendingRecord);
		}

		return table;
	}

	/// <summary>
	/// 文字列の末尾時点で、ダブルクォートで開始したフィールドの内側にいる(閉じるクォートが
	/// まだ現れていない)かどうかを判定する。
	/// </summary>
	private static bool EndsInsideQuotedField(string text)
	{
		var inQuotes = false;
		for (var i = 0; i < text.Length; i++)
		{
			if (text[i] != '"')
			{
				continue;
			}

			if (inQuotes && i + 1 < text.Length && text[i + 1] == '"')
			{
				i++; // エスケープされた""はスキップする
			}
			else
			{
				inQuotes = !inQuotes;
			}
		}

		return inQuotes;
	}

	/// <summary>
	/// ヘッダーに重複する列名がある場合、2件目以降に連番サフィックスを付けて一意化する。
	/// <see cref="CsvRow"/> は<see cref="Dictionary{TKey, TValue}"/>を継承しており、
	/// 重複キーのまま代入すると後勝ちで値が上書きされ列が実質的に消えてしまうため。
	/// </summary>
	private static List<string> DisambiguateHeaders(List<string> headers)
	{
		var occurrences = new Dictionary<string, int>();
		var result = new List<string>(headers.Count);

		foreach (var header in headers)
		{
			if (!occurrences.TryGetValue(header, out var count))
			{
				occurrences[header] = 1;
				result.Add(header);
			}
			else
			{
				count++;
				occurrences[header] = count;
				result.Add($"{header} ({count})");
			}
		}

		return result;
	}

	/// <summary>
	/// 1行分のCSVテキストをカンマ区切りのフィールドに分割する。
	/// ダブルクォートで囲まれたフィールド内のカンマは区切りとして扱わない。
	/// </summary>
	private static List<string> SplitLine(string line)
	{
		var fields = new List<string>();
		var current = new System.Text.StringBuilder();
		var inQuotes = false;

		for (var i = 0; i < line.Length; i++)
		{
			var c = line[i];

			if (inQuotes)
			{
				if (c == '"')
				{
					if (i + 1 < line.Length && line[i + 1] == '"')
					{
						current.Append('"');
						i++;
					}
					else
					{
						inQuotes = false;
					}
				}
				else
				{
					current.Append(c);
				}
			}
			else
			{
				if (c == '"')
				{
					inQuotes = true;
				}
				else if (c == ',')
				{
					fields.Add(current.ToString());
					current.Clear();
				}
				else
				{
					current.Append(c);
				}
			}
		}

		fields.Add(current.ToString());
		return fields;
	}

	/// <summary>
	/// <see cref="CsvTable"/> からCSVの各行(ヘッダー行を含む)を生成する。
	/// カンマ・ダブルクォート・改行を含む値はダブルクォートで囲む。
	/// </summary>
	public static IEnumerable<string> ToCsvLines(CsvTable table)
	{
		yield return string.Join(",", table.Headers.Select(EscapeField));

		foreach (var row in table.Rows)
		{
			yield return string.Join(",", table.Headers.Select(h => EscapeField(row.TryGetValue(h, out var v) ? v : string.Empty)));
		}
	}

	private static string EscapeField(string value)
	{
		if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
		{
			return $"\"{value.Replace("\"", "\"\"")}\"";
		}

		return value;
	}
}
