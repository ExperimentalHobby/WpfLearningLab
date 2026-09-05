using BehaviorGallery.AttachedProperties;

namespace BehaviorGallery.Tests;

/// <summary>
/// <see cref="PlaceholderVisibility"/> のテスト。
/// </summary>
public class PlaceholderVisibilityTests
{
    /// <summary>
    /// パス条件: テキストが空文字の場合、プレースホルダーを表示すべき(true)と判定すること
    /// </summary>
    [Fact]
    public void ShouldShow_テキストが空の場合はtrueを返す()
    {
        var result = PlaceholderVisibility.ShouldShow("");

        Assert.True(result);
    }

    /// <summary>
    /// パス条件: テキストが入力されている場合、プレースホルダーを表示すべきでない(false)と判定すること
    /// </summary>
    [Fact]
    public void ShouldShow_テキストがある場合はfalseを返す()
    {
        var result = PlaceholderVisibility.ShouldShow("山田太郎");

        Assert.False(result);
    }
}
