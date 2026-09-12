namespace ScreenCaptureTool.Services;

/// <summary>
/// 画面キャプチャ処理で発生したエラー(不正な範囲指定・GDI呼び出し失敗等)を表す。
/// </summary>
public class ScreenCaptureException : Exception
{
	/// <summary>
	/// <see cref="ScreenCaptureException"/>を初期化する。
	/// </summary>
	/// <param name="message">エラーメッセージ。</param>
	public ScreenCaptureException(string message) : base(message)
	{
	}

	/// <summary>
	/// <see cref="ScreenCaptureException"/>を初期化する。
	/// </summary>
	/// <param name="message">エラーメッセージ。</param>
	/// <param name="innerException">原因となった例外。</param>
	public ScreenCaptureException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
