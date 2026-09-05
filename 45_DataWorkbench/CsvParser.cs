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

        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            var fields = SplitLine(line);

            if (isFirstLine)
            {
                table.Headers.AddRange(fields);
                isFirstLine = false;
                continue;
            }

            var row = new CsvRow();
            for (var i = 0; i < table.Headers.Count; i++)
            {
                row[table.Headers[i]] = i < fields.Count ? fields[i] : string.Empty;
            }

            table.Rows.Add(row);
        }

        return table;
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
