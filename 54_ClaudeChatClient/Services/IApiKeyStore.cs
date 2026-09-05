namespace ClaudeChatClient.Services;

/// <summary>
/// 暗号化されたAPIキーの永続化を担う抽象。
/// </summary>
public interface IApiKeyStore
{
	/// <summary>
	/// 保存済みのレコードを読み込む。
	/// </summary>
	/// <param name="record">読み込めた場合はそのレコード。未初期化の場合は<see langword="null"/>。</param>
	/// <returns>保存済みの場合は<see langword="true"/>。</returns>
	bool TryLoad(out ApiKeyRecord? record);

	/// <summary>
	/// レコードを保存する。
	/// </summary>
	/// <param name="record">保存するレコード。</param>
	void Save(ApiKeyRecord record);
}
