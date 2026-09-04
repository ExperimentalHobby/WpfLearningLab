using SingleInstanceLauncher.Models;

namespace SingleInstanceLauncher.Tests;

/// <summary>
/// <see cref="LaunchMessageSerializer"/> のテスト。
/// </summary>
public class LaunchMessageSerializerTests
{
    /// <summary>
    /// パス条件: LaunchMessageをSerializeしてDeserializeすると、同じ内容が復元されること
    /// </summary>
    [Fact]
    public void Serialize_Deserialize_ラウンドトリップで同じ内容を復元できる()
    {
        var sentAt = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);
        var message = new LaunchMessage(new[] { "C:\\sample.txt", "--flag" }, sentAt);

        var json = LaunchMessageSerializer.Serialize(message);
        var restored = LaunchMessageSerializer.Deserialize(json);

        Assert.Equal(message.Arguments, restored.Arguments);
        Assert.Equal(message.SentAtUtc, restored.SentAtUtc);
    }

    /// <summary>
    /// パス条件: 起動引数が空配列のLaunchMessageをSerialize/Deserializeしても
    /// 空配列のまま正しく復元されること
    /// </summary>
    [Fact]
    public void Deserialize_引数が空配列の場合も正しく復元できる()
    {
        var message = new LaunchMessage(Array.Empty<string>(), DateTime.UtcNow);

        var json = LaunchMessageSerializer.Serialize(message);
        var restored = LaunchMessageSerializer.Deserialize(json);

        Assert.Empty(restored.Arguments);
    }
}
