using System.IO;
using System.Text.Json;

namespace ClaudeChatClient.Services;

/// <summary>
/// ローカルファイルにJSON形式で保存する<see cref="IApiKeyStore"/>の実装。
/// </summary>
/// <param name="filePath">保存先のファイルパス。</param>
public class FileApiKeyStore(string filePath) : IApiKeyStore
{
	private record StoredRecordDto(string Salt, string Verification, string EncryptedApiKey);

	/// <inheritdoc/>
	public bool TryLoad(out ApiKeyRecord? record)
	{
		if (!File.Exists(filePath))
		{
			record = null;
			return false;
		}

		try
		{
			var json = File.ReadAllText(filePath);
			var dto = JsonSerializer.Deserialize<StoredRecordDto>(json);
			if (dto is null)
			{
				record = null;
				return false;
			}

			record = new ApiKeyRecord(Convert.FromBase64String(dto.Salt), dto.Verification, dto.EncryptedApiKey);
			return true;
		}
		catch (Exception ex) when (ex is JsonException or IOException or UnauthorizedAccessException or FormatException)
		{
			// ファイル破損・権限エラー等は「未保存」として扱い、初回セットアップへ誘導する
			// (1件の破損が起動そのものを止めないようにする)。
			record = null;
			return false;
		}
	}

	/// <inheritdoc/>
	public void Save(ApiKeyRecord record)
	{
		var dto = new StoredRecordDto(Convert.ToBase64String(record.Salt), record.VerificationCipherText, record.EncryptedApiKey);

		try
		{
			var directory = Path.GetDirectoryName(filePath);
			if (!string.IsNullOrEmpty(directory))
			{
				Directory.CreateDirectory(directory);
			}

			File.WriteAllText(filePath, JsonSerializer.Serialize(dto));
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
		{
			throw new ApiKeyStoreException("APIキーの保存に失敗しました。保存先を確認してください。", ex);
		}
	}
}
