namespace ReactiveSearch.Services;

/// <summary>
/// 名前の重複チェック(サーバー問い合わせ相当の非同期処理)を抽象化する。
/// テスト時はFakeに差し替え、実運用では <see cref="DuplicateNameValidator"/> を使用する。
/// </summary>
public interface IDuplicateNameValidator
{
    /// <summary>
    /// 名前を検証する。予約済みの場合はエラーメッセージを、問題なければ null を返す。
    /// </summary>
    Task<string?> ValidateAsync(string name, CancellationToken cancellationToken);
}
