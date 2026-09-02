using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LogStreamAggregator.ViewModels;

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
	/// </summary>
	protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
