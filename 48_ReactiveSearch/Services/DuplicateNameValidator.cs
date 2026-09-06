namespace ReactiveSearch.Services;

/// <summary>
/// 名前が予約済み(使用済み)かどうかをサーバー問い合わせ相当の非同期処理でチェックする。
/// </summary>
public class DuplicateNameValidator : IDuplicateNameValidator
{
    private readonly HashSet<string> _reservedNames;
    private readonly TimeSpan _simulatedDelay;

    /// <param name="reservedNames">既に使用されている名前一覧(大文字小文字を区別しない)。</param>
    /// <param name="simulatedDelay">サーバー往復を模した遅延時間。テスト時は<see cref="TimeSpan.Zero"/>を指定する。</param>
    public DuplicateNameValidator(IEnumerable<string> reservedNames, TimeSpan simulatedDelay)
    {
        _reservedNames = new HashSet<string>(reservedNames, StringComparer.OrdinalIgnoreCase);
        _simulatedDelay = simulatedDelay;
    }

    /// <summary>
    /// 名前を検証する。予約済みの場合はエラーメッセージを、問題なければ null を返す。
    /// </summary>
    public async Task<string?> ValidateAsync(string name, CancellationToken cancellationToken)
    {
        if (_simulatedDelay > TimeSpan.Zero)
        {
            await Task.Delay(_simulatedDelay, cancellationToken);
        }

        return _reservedNames.Contains(name) ? "この名前は既に使用されています。" : null;
    }
}
