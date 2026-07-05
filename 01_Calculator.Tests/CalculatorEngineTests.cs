namespace Calculator.Tests;

/// <summary>
/// <see cref="CalculatorEngine"/> の四則演算・状態管理に関するテスト。
/// </summary>
public class CalculatorEngineTests
{
    /// <summary>
    /// パス条件: 何も入力していない初期状態で Display が "0" であること。
    /// </summary>
    [Fact]
    public void InitialDisplay_IsZero()
    {
        var engine = new CalculatorEngine();

        Assert.Equal("0", engine.Display);
    }

    /// <summary>
    /// パス条件: 数字を1つ入力すると、その数字がそのまま Display に反映されること。
    /// </summary>
    [Fact]
    public void InputDigit_SingleDigit_ReflectsInDisplay()
    {
        var engine = new CalculatorEngine();

        engine.InputDigit("5");

        Assert.Equal("5", engine.Display);
    }

    /// <summary>
    /// パス条件: 数字を複数回続けて入力すると、Display 上で連結表示されること。
    /// </summary>
    [Fact]
    public void InputDigit_MultipleDigits_ConcatenatesInDisplay()
    {
        var engine = new CalculatorEngine();

        engine.InputDigit("1");
        engine.InputDigit("2");
        engine.InputDigit("3");

        Assert.Equal("123", engine.Display);
    }

    /// <summary>
    /// パス条件: 演算子を入力した直後は、それまで入力した数字が Display に残ること。
    /// </summary>
    [Fact]
    public void InputOperator_RightAfterDigit_KeepsDisplayUnchanged()
    {
        var engine = new CalculatorEngine();

        engine.InputDigit("5");
        engine.InputOperator("+");

        Assert.Equal("5", engine.Display);
    }

    /// <summary>
    /// パス条件: 5 に 3 を足して = を押すと、Display が "8" になること。
    /// </summary>
    [Fact]
    public void InputEquals_Addition_ReturnsSum()
    {
        var engine = new CalculatorEngine();

        engine.InputDigit("5");
        engine.InputOperator("+");
        engine.InputDigit("3");
        engine.InputEquals();

        Assert.Equal("8", engine.Display);
    }

    /// <summary>
    /// パス条件: 5 から 3 を引いて = を押すと、Display が "2" になること。
    /// </summary>
    [Fact]
    public void InputEquals_Subtraction_ReturnsDifference()
    {
        var engine = new CalculatorEngine();

        engine.InputDigit("5");
        engine.InputOperator("-");
        engine.InputDigit("3");
        engine.InputEquals();

        Assert.Equal("2", engine.Display);
    }

    /// <summary>
    /// パス条件: 5 と 3 をかけて = を押すと、Display が "15" になること。
    /// </summary>
    [Fact]
    public void InputEquals_Multiplication_ReturnsProduct()
    {
        var engine = new CalculatorEngine();

        engine.InputDigit("5");
        engine.InputOperator("×");
        engine.InputDigit("3");
        engine.InputEquals();

        Assert.Equal("15", engine.Display);
    }

    /// <summary>
    /// パス条件: 6 を 3 で割って = を押すと、Display が "2" になること。
    /// </summary>
    [Fact]
    public void InputEquals_Division_ReturnsQuotient()
    {
        var engine = new CalculatorEngine();

        engine.InputDigit("6");
        engine.InputOperator("÷");
        engine.InputDigit("3");
        engine.InputEquals();

        Assert.Equal("2", engine.Display);
    }

    /// <summary>
    /// パス条件: ゼロで割って = を押すと、例外を投げずに Display が "Error" になること。
    /// </summary>
    [Fact]
    public void InputEquals_DivisionByZero_ShowsError()
    {
        var engine = new CalculatorEngine();

        engine.InputDigit("5");
        engine.InputOperator("÷");
        engine.InputDigit("0");
        engine.InputEquals();

        Assert.Equal("Error", engine.Display);
    }

    /// <summary>
    /// パス条件: 数字・演算子を入力した状態で Clear すると、初期状態(Display = "0")に戻ること。
    /// </summary>
    [Fact]
    public void Clear_ResetsDisplayToZero()
    {
        var engine = new CalculatorEngine();

        engine.InputDigit("5");
        engine.InputOperator("+");
        engine.InputDigit("3");
        engine.Clear();

        Assert.Equal("0", engine.Display);
    }

    /// <summary>
    /// パス条件: 演算子を連続で入力(+ の直後に -)すると、直前の演算子が上書きされ、
    /// 後から入力した演算子で計算されること。
    /// </summary>
    [Fact]
    public void InputOperator_ConsecutiveOperators_OverwritesPrevious()
    {
        var engine = new CalculatorEngine();

        engine.InputDigit("5");
        engine.InputOperator("+");
        engine.InputOperator("-");
        engine.InputDigit("3");
        engine.InputEquals();

        Assert.Equal("2", engine.Display);
    }

    /// <summary>
    /// パス条件: "5 + 3 + 2 =" のように演算子をはさんで連続入力すると、
    /// 直前の計算結果を引き継いで次の計算が行われること。
    /// </summary>
    [Fact]
    public void InputEquals_ChainedOperations_CarriesPreviousResult()
    {
        var engine = new CalculatorEngine();

        engine.InputDigit("5");
        engine.InputOperator("+");
        engine.InputDigit("3");
        engine.InputOperator("+");
        engine.InputDigit("2");
        engine.InputEquals();

        Assert.Equal("10", engine.Display);
    }

    /// <summary>
    /// パス条件: 数字入力後に小数点を入力すると、Display に "." が付加されること。
    /// </summary>
    [Fact]
    public void InputDecimalPoint_AfterDigit_AppendsDecimalPoint()
    {
        var engine = new CalculatorEngine();

        engine.InputDigit("3");
        engine.InputDecimalPoint();

        Assert.Equal("3.", engine.Display);
    }

    /// <summary>
    /// パス条件: 既に小数点がある状態で再度小数点を入力しても、2つ目の "." は追加されないこと。
    /// </summary>
    [Fact]
    public void InputDecimalPoint_WhenAlreadyPresent_DoesNotAddSecondPoint()
    {
        var engine = new CalculatorEngine();

        engine.InputDigit("3");
        engine.InputDecimalPoint();
        engine.InputDigit("5");
        engine.InputDecimalPoint();

        Assert.Equal("3.5", engine.Display);
    }

    /// <summary>
    /// パス条件: 演算子入力の直後(次の数字待ち状態)で小数点を入力すると、
    /// 新しい数値が "0." から始まること。
    /// </summary>
    [Fact]
    public void InputDecimalPoint_RightAfterOperator_StartsNewNumberFromZero()
    {
        var engine = new CalculatorEngine();

        engine.InputDigit("5");
        engine.InputOperator("+");
        engine.InputDecimalPoint();
        engine.InputDigit("3");

        Assert.Equal("0.3", engine.Display);
    }

    /// <summary>
    /// パス条件: 小数を含む値同士を加算すると、小数点以下を含む正しい結果が表示されること。
    /// </summary>
    [Fact]
    public void InputEquals_WithDecimalOperands_ReturnsDecimalResult()
    {
        var engine = new CalculatorEngine();

        engine.InputDigit("1");
        engine.InputDecimalPoint();
        engine.InputDigit("5");
        engine.InputOperator("+");
        engine.InputDigit("2");
        engine.InputDecimalPoint();
        engine.InputDigit("3");
        engine.InputEquals();

        Assert.Equal("3.8", engine.Display);
    }
}
