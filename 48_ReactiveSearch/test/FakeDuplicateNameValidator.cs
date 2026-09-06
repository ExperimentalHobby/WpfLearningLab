using ReactiveSearch.Services;

namespace ReactiveSearch.Tests;

/// <summary>
/// <see cref="IDuplicateNameValidator"/> のテスト用Fake。例外のスロー・呼び出しごとの
/// <see cref="CancellationToken"/>の記録が可能で、任意のタイミングで完了させられる。
/// </summary>
public class FakeDuplicateNameValidator : IDuplicateNameValidator
{
    private readonly List<TaskCompletionSource<string?>> _pendingCalls = new();

    /// <summary>スローする例外。設定すると<see cref="ValidateAsync"/>が例外で完了する。</summary>
    public Exception? ExceptionToThrow { get; set; }

    /// <summary>直近の呼び出しで渡された<see cref="CancellationToken"/>。</summary>
    public CancellationToken LastToken { get; private set; }

    /// <inheritdoc />
    public Task<string?> ValidateAsync(string name, CancellationToken cancellationToken)
    {
        LastToken = cancellationToken;

        if (ExceptionToThrow != null)
        {
            return Task.FromException<string?>(ExceptionToThrow);
        }

        var tcs = new TaskCompletionSource<string?>();
        cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken));
        _pendingCalls.Add(tcs);
        return tcs.Task;
    }

    /// <summary>
    /// これまでの保留中の呼び出しのうち、最後のものを指定した結果で完了させる。
    /// </summary>
    public void CompleteLast(string? result)
    {
        _pendingCalls[^1].TrySetResult(result);
    }
}
