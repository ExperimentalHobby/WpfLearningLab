namespace SystemTrayUtility.Services;

/// <summary>
/// Windows起動時の自動起動登録を担う抽象。
/// </summary>
public interface IStartupRegistrar
{
	/// <summary>現在、自動起動が登録されているかどうか。</summary>
	bool IsRegistered();

	/// <summary>自動起動を登録する。</summary>
	void Register();

	/// <summary>自動起動の登録を解除する。</summary>
	void Unregister();
}
