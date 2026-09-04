using FileOrganizer.Models;
using FileOrganizer.Services;

namespace FileOrganizer.Tests;

/// <summary>
/// <see cref="FileOrganizerService"/> の単体テスト。
/// テストごとに実の一時フォルダへファイルを作成し、実際のファイル移動を検証する。
/// </summary>
public class FileOrganizerServiceTests : IDisposable
{
	private readonly string _watchFolder;

	public FileOrganizerServiceTests()
	{
		_watchFolder = Path.Combine(Path.GetTempPath(), $"FileOrganizerTests_{Guid.NewGuid():N}");
		Directory.CreateDirectory(_watchFolder);
	}

	public void Dispose()
	{
		if (Directory.Exists(_watchFolder))
		{
			Directory.Delete(_watchFolder, recursive: true);
		}
	}

	/// <summary>
	/// パス条件: ルールに合致するファイルが移動先フォルダへ移動されること
	/// </summary>
	[Fact]
	public async Task OrganizeFileAsync_ルールに合致するファイルが移動される()
	{
		var filePath = Path.Combine(_watchFolder, "photo.jpg");
		File.WriteAllText(filePath, "dummy");
		var rules = new List<SortingRule> { new(".jpg", "Images") };
		var service = new FileOrganizerService();

		var result = await service.OrganizeFileAsync(filePath, _watchFolder, rules);

		Assert.True(result.Moved);
		var expectedPath = Path.Combine(_watchFolder, "Images", "photo.jpg");
		Assert.Equal(expectedPath, result.DestinationPath);
		Assert.True(File.Exists(expectedPath));
		Assert.False(File.Exists(filePath));
	}

	/// <summary>
	/// パス条件: 移動先フォルダが存在しない場合は自動的に作成されること
	/// </summary>
	[Fact]
	public async Task OrganizeFileAsync_移動先フォルダが無ければ作成される()
	{
		var filePath = Path.Combine(_watchFolder, "photo.jpg");
		File.WriteAllText(filePath, "dummy");
		var rules = new List<SortingRule> { new(".jpg", "Images") };
		var service = new FileOrganizerService();

		await service.OrganizeFileAsync(filePath, _watchFolder, rules);

		Assert.True(Directory.Exists(Path.Combine(_watchFolder, "Images")));
	}

	/// <summary>
	/// パス条件: ルールに合致しないファイルは移動されないこと
	/// </summary>
	[Fact]
	public async Task OrganizeFileAsync_ルールに合致しないファイルは移動されない()
	{
		var filePath = Path.Combine(_watchFolder, "readme.txt");
		File.WriteAllText(filePath, "dummy");
		var rules = new List<SortingRule> { new(".jpg", "Images") };
		var service = new FileOrganizerService();

		var result = await service.OrganizeFileAsync(filePath, _watchFolder, rules);

		Assert.False(result.Moved);
		Assert.True(File.Exists(filePath));
	}

	/// <summary>
	/// パス条件: 複数ファイルの一括振り分けができること
	/// </summary>
	[Fact]
	public async Task OrganizeExistingFilesAsync_複数ファイルが一括振り分けされる()
	{
		File.WriteAllText(Path.Combine(_watchFolder, "photo1.jpg"), "dummy");
		File.WriteAllText(Path.Combine(_watchFolder, "photo2.jpg"), "dummy");
		File.WriteAllText(Path.Combine(_watchFolder, "report.pdf"), "dummy");
		var rules = new List<SortingRule> { new(".jpg", "Images"), new(".pdf", "Documents") };
		var service = new FileOrganizerService();

		var results = await service.OrganizeExistingFilesAsync(_watchFolder, rules);

		Assert.Equal(3, results.Count);
		Assert.All(results, r => Assert.True(r.Moved));
		Assert.Equal(2, Directory.GetFiles(Path.Combine(_watchFolder, "Images")).Length);
		Assert.Single(Directory.GetFiles(Path.Combine(_watchFolder, "Documents")));
	}

	/// <summary>
	/// パス条件: 移動先に同名ファイルが既に存在する場合、例外を投げずエラーとして結果に記録されること
	/// </summary>
	[Fact]
	public async Task OrganizeFileAsync_移動先に同名ファイルがある場合エラーとして記録されクラッシュしない()
	{
		var filePath = Path.Combine(_watchFolder, "photo.jpg");
		File.WriteAllText(filePath, "dummy");
		var destinationDir = Path.Combine(_watchFolder, "Images");
		Directory.CreateDirectory(destinationDir);
		File.WriteAllText(Path.Combine(destinationDir, "photo.jpg"), "existing");
		var rules = new List<SortingRule> { new(".jpg", "Images") };
		var service = new FileOrganizerService();

		var result = await service.OrganizeFileAsync(filePath, _watchFolder, rules);

		Assert.False(result.Moved);
		Assert.NotNull(result.ErrorMessage);
	}
}
