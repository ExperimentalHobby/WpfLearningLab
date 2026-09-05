namespace SingleInstanceLauncher.Models;

/// <summary>
/// 2個目以降の起動から既存インスタンスへ送信する、起動引数のメッセージ。
/// </summary>
/// <param name="Arguments">起動時のコマンドライン引数。</param>
/// <param name="SentAtUtc">送信日時(UTC)。</param>
public record LaunchMessage(string[] Arguments, DateTime SentAtUtc);
