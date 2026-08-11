namespace LocalChatApp.Services;

/// <summary>
/// クライアントとしてサーバーに接続する処理の抽象。
/// </summary>
public interface IChatClient
{
	/// <summary>
	/// 指定したホスト・ポートのサーバーに接続する。
	/// </summary>
	/// <param name="host">接続先ホスト名またはIPアドレス。</param>
	/// <param name="port">接続先ポート番号。</param>
	Task<IChatConnection> ConnectAsync(string host, int port);
}
