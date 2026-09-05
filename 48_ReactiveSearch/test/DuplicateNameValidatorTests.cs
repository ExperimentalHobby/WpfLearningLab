using ReactiveSearch.Services;

namespace ReactiveSearch.Tests;

/// <summary>
/// <see cref="DuplicateNameValidator"/> のテスト。
/// </summary>
public class DuplicateNameValidatorTests
{
    /// <summary>
    /// パス条件: 予約済みの名前(大文字小文字を区別しない)を検証すると、エラーメッセージを返すこと
    /// </summary>
    [Fact]
    public async Task ValidateAsync_予約済みの名前の場合はエラーメッセージを返す()
    {
        var validator = new DuplicateNameValidator(new[] { "admin", "root" }, simulatedDelay: TimeSpan.Zero);

        var error = await validator.ValidateAsync("Admin", CancellationToken.None);

        Assert.Equal("この名前は既に使用されています。", error);
    }

    /// <summary>
    /// パス条件: 予約されていない名前を検証すると、nullを返すこと
    /// </summary>
    [Fact]
    public async Task ValidateAsync_未使用の名前の場合はnullを返す()
    {
        var validator = new DuplicateNameValidator(new[] { "admin", "root" }, simulatedDelay: TimeSpan.Zero);

        var error = await validator.ValidateAsync("yamada", CancellationToken.None);

        Assert.Null(error);
    }
}
