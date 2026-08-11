using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KanbanTaskManager.ViewModels;

/// <summary>
/// <see cref="INotifyPropertyChanged"/> を実装するViewModelの基底クラス。
/// </summary>
public abstract class ObservableObject : INotifyPropertyChanged
{
	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;

	/// <summary>
	/// フィールドの値を更新し、変更があった場合のみ <see cref="PropertyChanged"/> を発火する。
	/// </summary>
	/// <param name="field">バッキングフィールドへの参照。</param>
	/// <param name="value">設定する新しい値。</param>
	/// <param name="propertyName">通知するプロパティ名(呼び出し元から自動取得)。</param>
	/// <returns>値が変更された場合は <see langword="true"/>。</returns>
	protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
	{
		if (EqualityComparer<T>.Default.Equals(field, value))
		{
			return false;
		}

		field = value;
		OnPropertyChanged(propertyName);
		return true;
	}

	/// <summary>
	/// 指定したプロパティ名で <see cref="PropertyChanged"/> を発火する。
	/// フィールドを持たない算出プロパティの変更通知に使う。
	/// </summary>
	/// <param name="propertyName">通知するプロパティ名。</param>
	protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
