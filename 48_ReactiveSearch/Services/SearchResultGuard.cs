namespace ReactiveSearch.Services;

/// <summary>
/// 非同期処理完了時に「その結果がまだ最新のリクエストのものか」を判定するためのガード。
/// 新しいリクエストが始まると世代番号が進み、古い番号での判定は false になる
/// (古い検索結果の反映を防ぐ = 実質的なキャンセル)。
/// </summary>
public class SearchResultGuard
{
    private int _version;

    /// <summary>
    /// 新しいリクエストを開始し、そのリクエストの世代番号を発行する。
    /// </summary>
    public int BeginRequest()
    {
        return ++_version;
    }

    /// <summary>
    /// 指定した世代番号が最新のリクエストかどうかを判定する。
    /// </summary>
    public bool IsCurrent(int requestVersion)
    {
        return requestVersion == _version;
    }
}
