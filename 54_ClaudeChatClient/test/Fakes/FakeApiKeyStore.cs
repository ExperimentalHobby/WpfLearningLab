using ClaudeChatClient.Services;

namespace ClaudeChatClient.Tests.Fakes;

/// <summary>
/// テスト用の<see cref="IApiKeyStore"/>フェイク実装。メモリ上にのみ保持する。
/// </summary>
public class FakeApiKeyStore : IApiKeyStore
{
	private ApiKeyRecord? _record;

	/// <summary>
	/// 設定すると、次回のSave呼び出しでこの例外をスローする(異常系のテスト用)。
	/// </summary>
	public Exception? ExceptionToThrowOnSave { get; set; }

	public bool TryLoad(out ApiKeyRecord? record)
	{
		record = _record;
		return _record is not null;
	}

	public void Save(ApiKeyRecord record)
	{
		if (ExceptionToThrowOnSave is not null)
		{
			throw ExceptionToThrowOnSave;
		}

		_record = record;
	}
}
