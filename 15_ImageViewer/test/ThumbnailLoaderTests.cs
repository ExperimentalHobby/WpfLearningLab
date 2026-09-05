using ImageViewer.Services;

namespace ImageViewer.Tests;

/// <summary>
/// <see cref="ThumbnailLoader"/> の単体テスト。
/// </summary>
public class ThumbnailLoaderTests
{
	/// <summary>
	/// パス条件: 存在しないファイルパスを指定しても、例外を投げずnullを返すこと
	/// (破損画像・拡張子偽装ファイルでNotSupportedException/IOExceptionが送出される
	/// ケースを、フォルダ読込全体を止めずに1件だけスキップできるようにするため)
	/// </summary>
	[Fact]
	public async Task LoadAsync_存在しないファイルは例外を投げずnullを返す()
	{
		var loader = new ThumbnailLoader();

		var result = await loader.LoadAsync(@"C:\NonExistent_ThumbnailLoaderTest\dummy.png");

		Assert.Null(result);
	}

	/// <summary>
	/// パス条件: 画像として不正な内容のファイル(拡張子だけ画像を装ったテキストファイル)を
	/// 指定しても、例外を投げずnullを返すこと
	/// </summary>
	[Fact]
	public async Task LoadAsync_画像として不正な内容のファイルは例外を投げずnullを返す()
	{
		var tempPath = Path.Combine(Path.GetTempPath(), $"ThumbnailLoaderTest_{Guid.NewGuid():N}.jpg");
		await File.WriteAllTextAsync(tempPath, "これは画像ファイルではありません");
		try
		{
			var loader = new ThumbnailLoader();

			var result = await loader.LoadAsync(tempPath);

			Assert.Null(result);
		}
		finally
		{
			File.Delete(tempPath);
		}
	}
}
