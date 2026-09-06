using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BehaviorGallery.AttachedProperties;

/// <summary>
/// TextBoxにプレースホルダー文言を表示する添付プロパティ。
/// テキストが空の間だけ <see cref="VisualBrush"/> で描画した文言を
/// <see cref="Control.Background"/> に表示し、入力されると元に戻す。
/// </summary>
public static class PlaceholderService
{
    /// <summary>プレースホルダーとして表示する文言。</summary>
    public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(
        "Text",
        typeof(string),
        typeof(PlaceholderService),
        new PropertyMetadata(string.Empty, OnTextChanged));

    /// <summary>
    /// プレースホルダーを初めて表示する際に退避しておく、元々の<see cref="Control.Background"/>。
    /// 非表示に戻す際、<c>ClearValue</c>ではなくこの値へ戻すことで、TextBoxが独自に設定していた
    /// Backgroundを失わないようにする。未退避の場合は<see langword="null"/>のまま。
    /// </summary>
    private static readonly DependencyProperty OriginalBackgroundProperty = DependencyProperty.RegisterAttached(
        "OriginalBackground",
        typeof(PlaceholderBackgroundCapture),
        typeof(PlaceholderService));

    /// <summary>
    /// 直近に構築したプレースホルダー用<see cref="VisualBrush"/>のキャッシュ。
    /// 文言・サイズが変化していなければ再利用し、入力のたびに新規生成することによる
    /// GCプレッシャーを避ける。
    /// </summary>
    private static readonly DependencyProperty VisualCacheProperty = DependencyProperty.RegisterAttached(
        "VisualCache",
        typeof(PlaceholderVisualCache),
        typeof(PlaceholderService));

    public static string GetText(DependencyObject obj) => (string)obj.GetValue(TextProperty);

    public static void SetText(DependencyObject obj, string value) => obj.SetValue(TextProperty, value);

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBox textBox)
        {
            return;
        }

        // 値が変わるたびに一旦解除し、プレースホルダー文言が設定されている場合のみ再購読する。
        // (文言が空/未設定に戻された場合は購読したままにしない)
        textBox.TextChanged -= TextBox_TextChanged;
        if (!string.IsNullOrEmpty((string)e.NewValue))
        {
            textBox.TextChanged += TextBox_TextChanged;
        }

        UpdatePlaceholder(textBox);
    }

    private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            UpdatePlaceholder(textBox);
        }
    }

    private static void UpdatePlaceholder(TextBox textBox)
    {
        var placeholder = GetText(textBox);

        if (PlaceholderVisibility.ShouldShow(textBox.Text) && !string.IsNullOrEmpty(placeholder))
        {
            // プレースホルダーを初めて表示するタイミングで、その時点のBackgroundを退避しておく。
            if (textBox.GetValue(OriginalBackgroundProperty) is not PlaceholderBackgroundCapture)
            {
                textBox.SetValue(OriginalBackgroundProperty, new PlaceholderBackgroundCapture(textBox.Background));
            }

            var width = textBox.ActualWidth > 0 ? textBox.ActualWidth : 200;
            var height = textBox.ActualHeight > 0 ? textBox.ActualHeight : 24;

            var cache = textBox.GetValue(VisualCacheProperty) as PlaceholderVisualCache;
            if (cache is null || cache.Text != placeholder || cache.Width != width || cache.Height != height)
            {
                cache = BuildVisualCache(placeholder, width, height);
                textBox.SetValue(VisualCacheProperty, cache);
            }

            textBox.Background = cache.Brush;
        }
        else if (textBox.GetValue(OriginalBackgroundProperty) is PlaceholderBackgroundCapture capture)
        {
            // プレースホルダーを一度でも表示したことがある場合のみ、退避しておいた元のBackgroundへ戻す。
            textBox.Background = capture.Background;
        }
    }

    private static PlaceholderVisualCache BuildVisualCache(string placeholder, double width, double height)
    {
        var visual = new TextBlock
        {
            Text = placeholder,
            Foreground = Brushes.Gray,
            FontStyle = FontStyles.Italic,
            Margin = new Thickness(4, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Center,
        };

        visual.Measure(new Size(width, height));
        visual.Arrange(new Rect(0, 0, width, height));

        var brush = new VisualBrush(visual)
        {
            Stretch = Stretch.None,
            AlignmentX = AlignmentX.Left,
            TileMode = TileMode.None,
        };

        return new PlaceholderVisualCache(placeholder, width, height, brush);
    }

    /// <summary>
    /// 退避したBackgroundを保持するラッパー。<see cref="Brush"/>自体は<see langword="null"/>もありうるため、
    /// 「退避済みかどうか」を型の有無(<see langword="null"/>かどうか)で判定できるようにラップしている。
    /// </summary>
    private sealed record PlaceholderBackgroundCapture(Brush? Background);

    /// <summary>
    /// 構築済みのプレースホルダー用<see cref="VisualBrush"/>と、それを構築した際の文言・サイズ。
    /// </summary>
    private sealed record PlaceholderVisualCache(string Text, double Width, double Height, VisualBrush Brush);
}
