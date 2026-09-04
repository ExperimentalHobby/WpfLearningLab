using System.Text.Json;
using SingleInstanceLauncher.Models;

namespace SingleInstanceLauncher;

/// <summary>
/// <see cref="LaunchMessage"/> をNamed Pipesでやり取りするための1行JSON文字列に
/// 変換するロジック。
/// </summary>
public static class LaunchMessageSerializer
{
    /// <summary>LaunchMessageをJSON文字列にシリアライズする。</summary>
    public static string Serialize(LaunchMessage message)
    {
        return JsonSerializer.Serialize(message);
    }

    /// <summary>JSON文字列からLaunchMessageを復元する。</summary>
    public static LaunchMessage Deserialize(string json)
    {
        return JsonSerializer.Deserialize<LaunchMessage>(json)
            ?? throw new InvalidOperationException("LaunchMessageの復元に失敗しました。");
    }
}
