using ClaudeChatClient.Services;

namespace ClaudeChatClient.Tests.Fakes;

/// <summary>
/// テスト用の<see cref="IApiKeyStore"/>フェイク実装。メモリ上にのみ保持する。
/// </summary>
public class FakeApiKeyStore : IApiKeyStore
{
	private ApiKeyRecord? _record;

	public bool TryLoad(out ApiKeyRecord? record)
	{
		record = _record;
		return _record is not null;
	}

	public void Save(ApiKeyRecord record) => _record = record;
}
