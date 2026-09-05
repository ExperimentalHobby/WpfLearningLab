namespace DataWorkbench.Tests;

/// <summary>
/// <see cref="CsvParser"/> のテスト。
/// </summary>
public class CsvParserTests
{
    /// <summary>
    /// パス条件: ヘッダー行とデータ行を持つCSVをParseすると、HeadersとRowsに正しく分割されること
    /// </summary>
    [Fact]
    public void Parse_ヘッダーとデータ行を正しく分割できる()
    {
        var lines = new[]
        {
            "名前,年齢",
            "山田太郎,30",
            "鈴木花子,25",
        };

        var table = CsvParser.Parse(lines);

        Assert.Equal(new[] { "名前", "年齢" }, table.Headers);
        Assert.Equal(2, table.Rows.Count);
        Assert.Equal("山田太郎", table.Rows[0]["名前"]);
        Assert.Equal("30", table.Rows[0]["年齢"]);
        Assert.Equal("鈴木花子", table.Rows[1]["名前"]);
        Assert.Equal("25", table.Rows[1]["年齢"]);
    }

    /// <summary>
    /// パス条件: ダブルクォートで囲まれたフィールド内のカンマが区切り文字として扱われないこと
    /// </summary>
    [Fact]
    public void Parse_カンマを含む引用符付きフィールドを正しく解釈できる()
    {
        var lines = new[]
        {
            "名前,備考",
            "\"山田,太郎\",テスト",
        };

        var table = CsvParser.Parse(lines);

        Assert.Equal("山田,太郎", table.Rows[0]["名前"]);
        Assert.Equal("テスト", table.Rows[0]["備考"]);
    }

    /// <summary>
    /// パス条件: 空行を含むCSVをParseすると、空行は無視されデータ行として数えられないこと
    /// </summary>
    [Fact]
    public void Parse_空行は無視される()
    {
        var lines = new[]
        {
            "名前,年齢",
            "",
            "山田太郎,30",
            "",
        };

        var table = CsvParser.Parse(lines);

        Assert.Single(table.Rows);
    }

    /// <summary>
    /// パス条件: CsvTableからToCsvLinesを呼ぶと、ヘッダー行とデータ行が正しく生成されること
    /// </summary>
    [Fact]
    public void ToCsvLines_ヘッダーと行から正しいCSV文字列を生成する()
    {
        var table = CsvParser.Parse(new[] { "名前,年齢", "山田太郎,30" });

        var lines = CsvParser.ToCsvLines(table).ToList();

        Assert.Equal(new[] { "名前,年齢", "山田太郎,30" }, lines);
    }

    /// <summary>
    /// パス条件: カンマを含む値をToCsvLinesで出力すると、ダブルクォートで囲まれること
    /// </summary>
    [Fact]
    public void ToCsvLines_カンマを含む値はダブルクォートで囲む()
    {
        var table = CsvParser.Parse(new[] { "名前,備考", "\"山田,太郎\",テスト" });

        var lines = CsvParser.ToCsvLines(table).ToList();

        Assert.Equal("\"山田,太郎\",テスト", lines[1]);
    }
}
