using System.Globalization;

namespace Calculator;

public class CalculatorEngine
{
    public string Display { get; private set; } = "0";

    private double? _accumulator;
    private string? _pendingOperator;
    private bool _shouldResetDisplayOnNextDigit;

    public void InputDigit(string digit)
    {
        if (_shouldResetDisplayOnNextDigit || Display == "0")
        {
            Display = digit;
            _shouldResetDisplayOnNextDigit = false;
        }
        else
        {
            Display += digit;
        }
    }

    public void InputOperator(string op)
    {
        if (_pendingOperator != null && !_shouldResetDisplayOnNextDigit)
        {
            Compute();
        }

        _accumulator = double.Parse(Display, CultureInfo.InvariantCulture);
        _pendingOperator = op;
        _shouldResetDisplayOnNextDigit = true;
    }

    public void InputEquals()
    {
        Compute();
        _pendingOperator = null;
    }

    public void Clear()
    {
        Display = "0";
        _accumulator = null;
        _pendingOperator = null;
        _shouldResetDisplayOnNextDigit = false;
    }

    private void Compute()
    {
        if (_pendingOperator == null || _accumulator == null)
        {
            return;
        }

        var operand = double.Parse(Display, CultureInfo.InvariantCulture);

        if (_pendingOperator == "÷" && operand == 0)
        {
            Display = "Error";
            _accumulator = null;
            _pendingOperator = null;
            _shouldResetDisplayOnNextDigit = true;
            return;
        }

        var result = _pendingOperator switch
        {
            "+" => _accumulator.Value + operand,
            "-" => _accumulator.Value - operand,
            "×" => _accumulator.Value * operand,
            "÷" => _accumulator.Value / operand,
            _ => operand,
        };

        Display = result.ToString(CultureInfo.InvariantCulture);
        _accumulator = result;
        _shouldResetDisplayOnNextDigit = true;
    }
}
