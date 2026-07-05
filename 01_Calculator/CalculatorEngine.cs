using System.Globalization;

namespace Calculator;

/// <summary>
/// 電卓の状態(入力中の数値・保留中の演算子・メモリ)を管理し、四則演算を実行するクラス。
/// UI(コードビハインド)からはボタン操作に対応するメソッドを呼び出すだけで済むようにする。
/// </summary>
public class CalculatorEngine
{
    /// <summary>
    /// 現在の表示文字列。数値入力中の値、または直前の計算結果、あるいは "Error" を保持する。
    /// </summary>
    public string Display { get; private set; } = "0";

    // 金額・小数計算で double を使うと 0.6 - 0.2 が 0.39999999999999997 のような
    // 二進浮動小数点誤差を起こすため、10進数で正確に計算できる decimal を採用する。
    private decimal? _accumulator;
    private string? _pendingOperator;
    private bool _shouldResetDisplayOnNextDigit;
    private decimal _memory;

    /// <summary>
    /// 数字ボタンの入力を受け取り、Display に反映する。
    /// 演算子入力直後やクリア直後は新しい数値の先頭として扱う。
    /// </summary>
    /// <param name="digit">入力された数字1文字("0"〜"9")。</param>
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

    /// <summary>
    /// 小数点ボタンの入力を受け取る。既に小数点を含む場合は何もしない。
    /// 演算子入力直後に押された場合は "0." から新しい数値を開始する。
    /// </summary>
    public void InputDecimalPoint()
    {
        if (_shouldResetDisplayOnNextDigit)
        {
            Display = "0.";
            _shouldResetDisplayOnNextDigit = false;
            return;
        }

        if (!Display.Contains('.'))
        {
            Display += ".";
        }
    }

    /// <summary>
    /// 演算子ボタン(+ - × ÷)の入力を受け取る。
    /// 既に保留中の演算子がある場合は先に計算を実行してから、新しい演算子を保留する。
    /// 演算子を連続で押した場合は直前の演算子を上書きする。
    /// </summary>
    /// <param name="op">入力された演算子("+", "-", "×", "÷")。</param>
    public void InputOperator(string op)
    {
        // 演算子を連続で押した(例: + の直後に -)場合は shouldResetDisplayOnNextDigit が
        // 立ったままになっているため Compute() を呼ばず、pendingOperator の上書きだけ行う。
        if (_pendingOperator != null && !_shouldResetDisplayOnNextDigit)
        {
            Compute();
        }

        _accumulator = decimal.Parse(Display, CultureInfo.InvariantCulture);
        _pendingOperator = op;
        _shouldResetDisplayOnNextDigit = true;
    }

    /// <summary>
    /// = ボタンの入力を受け取り、保留中の演算子で計算を確定する。
    /// </summary>
    public void InputEquals()
    {
        Compute();
        _pendingOperator = null;
    }

    /// <summary>
    /// C(クリア)ボタンの入力を受け取り、電卓の状態を初期状態に戻す。メモリの値は保持する。
    /// </summary>
    public void Clear()
    {
        Display = "0";
        _accumulator = null;
        _pendingOperator = null;
        _shouldResetDisplayOnNextDigit = false;
    }

    /// <summary>
    /// M+ ボタンの入力を受け取り、現在の Display の値をメモリに加算する。
    /// </summary>
    public void MemoryAdd()
    {
        _memory += decimal.Parse(Display, CultureInfo.InvariantCulture);
        _shouldResetDisplayOnNextDigit = true;
    }

    /// <summary>
    /// M- ボタンの入力を受け取り、現在の Display の値をメモリから減算する。
    /// </summary>
    public void MemorySubtract()
    {
        _memory -= decimal.Parse(Display, CultureInfo.InvariantCulture);
        _shouldResetDisplayOnNextDigit = true;
    }

    /// <summary>
    /// MR ボタンの入力を受け取り、メモリの値を Display に呼び出す。
    /// 呼び出し後に数字を入力すると、続きではなく新しい数値として上書きされる。
    /// </summary>
    public void MemoryRecall()
    {
        Display = _memory.ToString(CultureInfo.InvariantCulture);
        _shouldResetDisplayOnNextDigit = true;
    }

    /// <summary>
    /// MC ボタンの入力を受け取り、メモリの値を 0 にリセットする。
    /// </summary>
    public void MemoryClear()
    {
        _memory = 0;
    }

    /// <summary>
    /// 保留中の演算子と現在の Display の値をもとに計算を実行し、結果を Display に反映する。
    /// ゼロ除算の場合は例外を投げず Display を "Error" にする。
    /// </summary>
    private void Compute()
    {
        if (_pendingOperator == null || _accumulator == null)
        {
            return;
        }

        var operand = decimal.Parse(Display, CultureInfo.InvariantCulture);

        // decimal の除算はゼロ除算で DivideByZeroException を送出するため、
        // 例外に頼らず先に判定して "Error" 表示に倒す。
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
