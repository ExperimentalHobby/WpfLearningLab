using System.Windows.Controls;
using System.Windows.Media;
using BehaviorGallery.AttachedProperties;

namespace BehaviorGallery.Tests;

/// <summary>
/// <see cref="PlaceholderService"/> のテスト。実際の<see cref="TextBox"/>を使って検証する。
/// </summary>
public class PlaceholderServiceTests
{
    /// <summary>
    /// パス条件: プレースホルダー文言を設定した未入力のTextBoxは、BackgroundがVisualBrushになること
    /// </summary>
    [WpfFact]
    public void Text設定時_未入力のTextBoxはBackgroundがVisualBrushになる()
    {
        var textBox = new TextBox();

        PlaceholderService.SetText(textBox, "氏名を入力してください");

        Assert.IsType<VisualBrush>(textBox.Background);
    }

    /// <summary>
    /// パス条件: 独自のBackgroundを持つTextBoxにプレースホルダーを設定した状態でテキストを入力すると、
    /// Backgroundが既定値ではなく元の独自の値に復元されること
    /// (ClearValueで既定値に戻ってしまい、元のBackgroundが失われる不具合の回帰テスト)。
    /// </summary>
    [WpfFact]
    public void 入力時_元の独自のBackgroundに復元される()
    {
        var originalBackground = Brushes.LightYellow;
        var textBox = new TextBox { Background = originalBackground };

        PlaceholderService.SetText(textBox, "氏名を入力してください");
        textBox.Text = "山田太郎";

        Assert.Same(originalBackground, textBox.Background);
    }

    /// <summary>
    /// パス条件: プレースホルダー文言を空に戻した後にテキストを変更しても、
    /// プレースホルダー機能によるBackground操作が行われないこと
    /// (TextChangedの購読解除経路が無い不具合の回帰テスト)。
    /// </summary>
    [WpfFact]
    public void Textを空に戻した後は_Background操作が行われない()
    {
        var textBox = new TextBox();
        PlaceholderService.SetText(textBox, "氏名を入力してください");
        PlaceholderService.SetText(textBox, string.Empty);
        textBox.Background = Brushes.Pink;

        textBox.Text = "何か入力";
        textBox.Text = string.Empty;

        Assert.Same(Brushes.Pink, textBox.Background);
    }

    /// <summary>
    /// パス条件: 同じプレースホルダー文言・同じサイズで表示/非表示を繰り返しても、
    /// VisualBrushインスタンスが再生成されず使い回されること
    /// (入力のたびにTextBlock/VisualBrushを新規生成しGCプレッシャーになっていた不具合の回帰テスト)。
    /// </summary>
    [WpfFact]
    public void 表示を繰り返しても同じVisualBrushインスタンスが再利用される()
    {
        var textBox = new TextBox();
        PlaceholderService.SetText(textBox, "氏名を入力してください");
        var firstBrush = textBox.Background;

        textBox.Text = "山田太郎";
        textBox.Text = string.Empty;
        var secondBrush = textBox.Background;

        Assert.Same(firstBrush, secondBrush);
    }
}
