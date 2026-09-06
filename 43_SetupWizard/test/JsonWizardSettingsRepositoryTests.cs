using System.Text.Json;
using SetupWizard.Models;
using SetupWizard.Services;

namespace SetupWizard.Tests;

/// <summary>
/// <see cref="JsonWizardSettingsRepository"/> のテスト。
/// </summary>
public class JsonWizardSettingsRepositoryTests : IDisposable
{
	private readonly string _filePath;

	public JsonWizardSettingsRepositoryTests()
	{
		_filePath = Path.Combine(Path.GetTempPath(), $"SetupWizardTests_{Guid.NewGuid():N}.json");
	}

	public void Dispose()
	{
		if (File.Exists(_filePath))
		{
			File.Delete(_filePath);
		}
	}

	/// <summary>
	/// パス条件: Saveした内容が、JSONファイルとして正しく書き込まれること
	/// (「完了」ボタンを押しても入力内容が保存されないクラッシュ級ではないが実害のある不具合の回帰テスト)。
	/// </summary>
	[Fact]
	public void Save_入力内容がJSONファイルに書き込まれる()
	{
		var repository = new JsonWizardSettingsRepository(_filePath);
		var state = new WizardState
		{
			Name = "山田太郎",
			Email = "taro@example.com",
			Department = "開発",
			EnableNotifications = true,
			Comment = "コメント",
		};

		repository.Save(state);

		Assert.True(File.Exists(_filePath));
		var saved = JsonSerializer.Deserialize<WizardState>(File.ReadAllText(_filePath));
		Assert.Equal("山田太郎", saved?.Name);
		Assert.Equal("taro@example.com", saved?.Email);
		Assert.Equal("開発", saved?.Department);
		Assert.True(saved?.EnableNotifications);
		Assert.Equal("コメント", saved?.Comment);
	}

	/// <summary>
	/// パス条件: 保存先フォルダが存在しない場合でも、自動的に作成した上でSaveできること
	/// </summary>
	[Fact]
	public void Save_保存先フォルダが存在しない場合は作成される()
	{
		var nestedPath = Path.Combine(Path.GetTempPath(), $"SetupWizardTests_{Guid.NewGuid():N}", "wizard-settings.json");
		var repository = new JsonWizardSettingsRepository(nestedPath);

		repository.Save(new WizardState { Name = "山田太郎" });

		Assert.True(File.Exists(nestedPath));
		Directory.Delete(Path.GetDirectoryName(nestedPath)!, recursive: true);
	}
}
