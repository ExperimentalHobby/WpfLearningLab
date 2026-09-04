namespace SystemTrayUtility.Services;

/// <summary>
/// 定期リマインダーの間隔入力(分単位)を検証・変換する。
/// </summary>
public static class ReminderIntervalParser
{
	/// <summary>
	/// 入力文字列を正の整数分として解析し、<see cref="TimeSpan"/>に変換する。
	/// 0以下・数値でない場合は<see langword="false"/>を返す。
	/// </summary>
	public static bool TryParseMinutes(string input, out TimeSpan interval)
	{
		interval = TimeSpan.Zero;
		if (!int.TryParse(input, out var minutes) || minutes <= 0)
		{
			return false;
		}
		interval = TimeSpan.FromMinutes(minutes);
		return true;
	}
}
