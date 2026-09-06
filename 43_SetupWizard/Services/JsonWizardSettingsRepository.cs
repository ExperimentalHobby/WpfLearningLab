using System.IO;
using System.Text.Json;
using SetupWizard.Models;

namespace SetupWizard.Services;

/// <summary>
/// ウィザードの入力内容を1つのJSONファイルに保存する実装。
/// </summary>
public sealed class JsonWizardSettingsRepository : IWizardSettingsRepository
{
	private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

	private readonly string _filePath;

	/// <summary>
	/// 保存先ファイルのパスを指定して初期化する。
	/// </summary>
	/// <param name="filePath">保存先ファイルのパス。</param>
	public JsonWizardSettingsRepository(string filePath)
	{
		_filePath = filePath;
	}

	/// <inheritdoc/>
	public void Save(WizardState state)
	{
		var directory = Path.GetDirectoryName(_filePath);
		if (!string.IsNullOrEmpty(directory))
		{
			Directory.CreateDirectory(directory);
		}

		File.WriteAllText(_filePath, JsonSerializer.Serialize(state, SerializerOptions));
	}
}
