using System.Windows;
using System.Windows.Controls;

namespace Calculator;

/// <summary>
/// 電卓アプリのメインウィンドウ。ボタンのClickイベントを <see cref="CalculatorEngine"/> に委譲し、
/// 計算結果を表示欄(DisplayText)に反映するだけの薄いコードビハインド。
/// </summary>
public partial class MainWindow : Window
{
    private readonly CalculatorEngine _engine = new();

    /// <summary>
    /// ウィンドウを初期化する。
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 数字ボタン(0〜9)がクリックされたときの処理。
    /// </summary>
    private void DigitButton_Click(object sender, RoutedEventArgs e)
    {
        var digit = (string)((Button)sender).Content;
        _engine.InputDigit(digit);
        DisplayText.Text = _engine.Display;
    }

    /// <summary>
    /// 小数点ボタン(.)がクリックされたときの処理。
    /// </summary>
    private void DecimalPointButton_Click(object sender, RoutedEventArgs e)
    {
        _engine.InputDecimalPoint();
        DisplayText.Text = _engine.Display;
    }

    /// <summary>
    /// 演算子ボタン(+ - × ÷)がクリックされたときの処理。
    /// </summary>
    private void OperatorButton_Click(object sender, RoutedEventArgs e)
    {
        var op = (string)((Button)sender).Content;
        _engine.InputOperator(op);
        DisplayText.Text = _engine.Display;
    }

    /// <summary>
    /// = ボタンがクリックされたときの処理。
    /// </summary>
    private void EqualsButton_Click(object sender, RoutedEventArgs e)
    {
        _engine.InputEquals();
        DisplayText.Text = _engine.Display;
    }

    /// <summary>
    /// C(クリア)ボタンがクリックされたときの処理。
    /// </summary>
    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        _engine.Clear();
        DisplayText.Text = _engine.Display;
    }

    /// <summary>
    /// M+ ボタンがクリックされたときの処理。表示中の値をメモリに加算する。
    /// </summary>
    private void MemoryAddButton_Click(object sender, RoutedEventArgs e)
    {
        _engine.MemoryAdd();
    }

    /// <summary>
    /// M- ボタンがクリックされたときの処理。表示中の値をメモリから減算する。
    /// </summary>
    private void MemorySubtractButton_Click(object sender, RoutedEventArgs e)
    {
        _engine.MemorySubtract();
    }

    /// <summary>
    /// MR ボタンがクリックされたときの処理。メモリの値を表示欄に呼び出す。
    /// </summary>
    private void MemoryRecallButton_Click(object sender, RoutedEventArgs e)
    {
        _engine.MemoryRecall();
        DisplayText.Text = _engine.Display;
    }

    /// <summary>
    /// MC ボタンがクリックされたときの処理。メモリの値を0にリセットする。
    /// </summary>
    private void MemoryClearButton_Click(object sender, RoutedEventArgs e)
    {
        _engine.MemoryClear();
    }
}
