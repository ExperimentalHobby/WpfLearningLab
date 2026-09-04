namespace BehaviorGallery.AttachedProperties;

/// <summary>
/// プレースホルダーを表示すべきかどうかの判定ロジック。
/// UI(Adorner/VisualBrush等の描画)から切り出した純粋な判定部分。
/// </summary>
public static class PlaceholderVisibility
{
    /// <summary>
    /// テキストが空の場合にプレースホルダーを表示すべきかどうかを判定する。
    /// </summary>
    public static bool ShouldShow(string? text)
    {
        return string.IsNullOrEmpty(text);
    }
}
