using DataWorkbench.Models;

namespace DataWorkbench;

/// <summary>
/// 検索文字列によるCSV行のフィルタ判定ロジック。
/// </summary>
public static class CsvFilterEngine
{
    /// <summary>
    /// 検索文字列が行のいずれかの列の値に部分一致(大文字小文字を区別しない)するかを判定する。
    /// 検索文字列が空の場合は常にtrueを返す(フィルタなし)。
    /// </summary>
    public static bool Matches(CsvRow row, string searchText)
    {
        if (string.IsNullOrEmpty(searchText))
        {
            return true;
        }

        return row.Values.Any(v => v.Contains(searchText, StringComparison.CurrentCultureIgnoreCase));
    }
}
