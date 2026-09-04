namespace DataWorkbench.Models;

/// <summary>
/// CSVの1行を表す。列数が可変なCSVを動的にDataGridへバインドするため、
/// WPFのBindingがDictionaryのインデクサ(<c>[列名]</c>)を解決できる性質を利用する。
/// </summary>
public class CsvRow : Dictionary<string, string>
{
}

/// <summary>
/// CSVファイル全体(ヘッダーと行データ)を表す。
/// </summary>
public class CsvTable
{
    /// <summary>列名一覧(出現順)。</summary>
    public List<string> Headers { get; } = new();

    /// <summary>データ行一覧。</summary>
    public List<CsvRow> Rows { get; } = new();
}
