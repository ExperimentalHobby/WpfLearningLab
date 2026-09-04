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

    public static string GetText(DependencyObject obj) => (string)obj.GetValue(TextProperty);

    public static void SetText(DependencyObject obj, string value) => obj.SetValue(TextProperty, value);

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBox textBox)
        {
            return;
        }

        textBox.TextChanged -= TextBox_TextChanged;
        textBox.TextChanged += TextBox_TextChanged;
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
            var visual = new TextBlock
            {
                Text = placeholder,
                Foreground = Brushes.Gray,
                FontStyle = FontStyles.Italic,
                Margin = new Thickness(4, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
            };

            var width = textBox.ActualWidth > 0 ? textBox.ActualWidth : 200;
            var height = textBox.ActualHeight > 0 ? textBox.ActualHeight : 24;
            visual.Measure(new Size(width, height));
            visual.Arrange(new Rect(0, 0, width, height));

            textBox.Background = new VisualBrush(visual)
            {
                Stretch = Stretch.None,
                AlignmentX = AlignmentX.Left,
                TileMode = TileMode.None,
            };
        }
        else
        {
            textBox.ClearValue(Control.BackgroundProperty);
        }
    }
}
