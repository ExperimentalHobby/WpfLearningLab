using SetupWizard.Models;

namespace SetupWizard.Services;

/// <summary>
/// ウィザードの入力内容の永続化を抽象化する。
/// </summary>
public interface IWizardSettingsRepository
{
	/// <summary>
	/// 入力内容を保存する。
	/// </summary>
	/// <param name="state">保存するウィザードの状態。</param>
	void Save(WizardState state);
}
