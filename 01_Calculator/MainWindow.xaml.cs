using System.Windows;
using System.Windows.Controls;

namespace Calculator;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly CalculatorEngine _engine = new();

    public MainWindow()
    {
        InitializeComponent();
    }

    private void DigitButton_Click(object sender, RoutedEventArgs e)
    {
        var digit = (string)((Button)sender).Content;
        _engine.InputDigit(digit);
        DisplayText.Text = _engine.Display;
    }

    private void OperatorButton_Click(object sender, RoutedEventArgs e)
    {
        var op = (string)((Button)sender).Content;
        _engine.InputOperator(op);
        DisplayText.Text = _engine.Display;
    }

    private void EqualsButton_Click(object sender, RoutedEventArgs e)
    {
        _engine.InputEquals();
        DisplayText.Text = _engine.Display;
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        _engine.Clear();
        DisplayText.Text = _engine.Display;
    }
}
