namespace LocalChatApp.Services;

/// <summary>
/// サーバーとしてクライアントの接続を待ち受ける処理の抽象。
/// </summary>
public interface IChatServer
{
	/// <summary>
	/// 指定したポートで待ち受け、1件のクライアント接続を確立する。
	/// </summary>
	/// <param name="port">待ち受けるポート番号。</param>
	Task<IChatConnection> WaitForConnectionAsync(int port);
}
