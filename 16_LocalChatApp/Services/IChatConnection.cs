namespace LocalChatApp.Services;

/// <summary>
/// 確立済みのチャット接続の抽象。1行のテキストメッセージ単位で送受信する。
/// </summary>
public interface IChatConnection : IDisposable
{
	/// <summary>相手からメッセージを受信したときに発火する。イベントは受信を担うバックグラウンドスレッドから発火する。</summary>
	event Action<string>? MessageReceived;

	/// <summary>接続が切断された(相手が閉じた、または通信エラーが発生した)ときに発火する。</summary>
	event Action? Disconnected;

	/// <summary>
	/// メッセージを1行のテキストとして送信する。
	/// </summary>
	Task SendAsync(string message);

	/// <summary>
	/// 接続を閉じる。
	/// </summary>
	void Close();
}
