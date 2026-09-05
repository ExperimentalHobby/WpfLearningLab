namespace ReactiveSearch.Services;

/// <summary>
/// 候補一覧から部分一致検索を行うシンプルな検索ロジック。
/// </summary>
public class SearchService
{
    private readonly IReadOnlyList<string> _candidates;

    public SearchService(IReadOnlyList<string> candidates)
    {
        _candidates = candidates;
    }

    /// <summary>
    /// 検索文字列に部分一致(大文字小文字を区別しない)する候補を返す。
    /// </summary>
    public IReadOnlyList<string> Search(string query)
    {
        if (string.IsNullOrEmpty(query))
        {
            return _candidates;
        }

        return _candidates
            .Where(c => c.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}
