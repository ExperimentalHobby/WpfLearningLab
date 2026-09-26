using ClaudeChatClient.Services;

namespace ClaudeChatClient.Tests;

/// <summary>
/// <see cref="FileApiKeyStore"/>の単体テスト。
/// </summary>
public class FileApiKeyStoreTests
{
	/// <summary>
	/// パス条件: 保存先ファイルの中身が破損したJSONの場合、TryLoadが例外を投げず
	/// falseを返すこと(未保存として扱う既存の契約と整合させる)。
	/// </summary>
	[Fact]
	public void TryLoad_CorruptedJson_ReturnsFalseWithoutThrowing()
	{
		var filePath = Path.Combine(Path.GetTempPath(), $"apikey-corrupted-{Guid.NewGuid()}.json");
		File.WriteAllText(filePath, "{ this is not valid json");
		try
		{
			var store = new FileApiKeyStore(filePath);

			var loaded = store.TryLoad(out var record);

			Assert.False(loaded);
			Assert.Null(record);
		}
		finally
		{
			File.Delete(filePath);
		}
	}

	/// <summary>
	/// パス条件: 保存先ディレクトリを作成できない場合、SaveがApiKeyStoreExceptionを投げること。
	/// </summary>
	[Fact]
	public void Save_DirectoryCannotBeCreated_ThrowsApiKeyStoreException()
	{
		// 保存先の「ディレクトリ」部分と同じパスに、あらかじめ普通のファイルを作っておくことで、
		// Directory.CreateDirectoryが失敗する状況(IOException)を再現する。
		var blockingFilePath = Path.Combine(Path.GetTempPath(), $"apikey-blocking-{Guid.NewGuid()}");
		File.WriteAllText(blockingFilePath, "dummy");
		try
		{
			var filePath = Path.Combine(blockingFilePath, "apikey.json");
			var store = new FileApiKeyStore(filePath);
			var record = new ApiKeyRecord([1, 2, 3], "verification", "encrypted");

			Assert.Throws<ApiKeyStoreException>(() => store.Save(record));
		}
		finally
		{
			File.Delete(blockingFilePath);
		}
	}
}
