namespace Calculator.Tests;

public class CalculatorEngineTests
{
    [Fact]
    public void 初期表示は0()
    {
        var engine = new CalculatorEngine();

        Assert.Equal("0", engine.Display);
    }

    [Fact]
    public void 数字を入力すると表示に反映される()
    {
        var engine = new CalculatorEngine();

        engine.InputDigit("5");

        Assert.Equal("5", engine.Display);
    }

    [Fact]
    public void 複数の数字を続けて入力すると連結表示される()
    {
        var engine = new CalculatorEngine();

        engine.InputDigit("1");
        engine.InputDigit("2");
        engine.InputDigit("3");

        Assert.Equal("123", engine.Display);
    }

    [Fact]
    public void 演算子入力直後は表示がそれまでの入力を保持する()
    {
        var engine = new CalculatorEngine();

        engine.InputDigit("5");
        engine.InputOperator("+");

        Assert.Equal("5", engine.Display);
    }

    [Fact]
    public void 加算_5と3を足すと8になる()
    {
        var engine = new CalculatorEngine();

        engine.InputDigit("5");
        engine.InputOperator("+");
        engine.InputDigit("3");
        engine.InputEquals();

        Assert.Equal("8", engine.Display);
    }

    [Fact]
    public void 減算_5から3を引くと2になる()
    {
        var engine = new CalculatorEngine();

        engine.InputDigit("5");
        engine.InputOperator("-");
        engine.InputDigit("3");
        engine.InputEquals();

        Assert.Equal("2", engine.Display);
    }

    [Fact]
    public void 乗算_5と3をかけると15になる()
    {
        var engine = new CalculatorEngine();

        engine.InputDigit("5");
        engine.InputOperator("×");
        engine.InputDigit("3");
        engine.InputEquals();

        Assert.Equal("15", engine.Display);
    }

    [Fact]
    public void 除算_6を3で割ると2になる()
    {
        var engine = new CalculatorEngine();

        engine.InputDigit("6");
        engine.InputOperator("÷");
        engine.InputDigit("3");
        engine.InputEquals();

        Assert.Equal("2", engine.Display);
    }

    [Fact]
    public void 除算_ゼロで割るとErrorが表示される()
    {
        var engine = new CalculatorEngine();

        engine.InputDigit("5");
        engine.InputOperator("÷");
        engine.InputDigit("0");
        engine.InputEquals();

        Assert.Equal("Error", engine.Display);
    }

    [Fact]
    public void クリアすると初期表示に戻る()
    {
        var engine = new CalculatorEngine();

        engine.InputDigit("5");
        engine.InputOperator("+");
        engine.InputDigit("3");
        engine.Clear();

        Assert.Equal("0", engine.Display);
    }

    [Fact]
    public void 連続演算子入力では直前の演算子が上書きされる()
    {
        var engine = new CalculatorEngine();

        engine.InputDigit("5");
        engine.InputOperator("+");
        engine.InputOperator("-");
        engine.InputDigit("3");
        engine.InputEquals();

        Assert.Equal("2", engine.Display);
    }

    [Fact]
    public void 連続した演算では前の計算結果を引き継ぐ()
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
}
