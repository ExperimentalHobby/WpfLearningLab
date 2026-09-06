using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SystemTrayUtility.ViewModels;

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
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		return true;
	}

	/// <summary>
	/// フィールドの値を変更せずに<see cref="PropertyChanged"/>のみを発火する。
	/// レジストリ登録失敗時など、内部状態を変更前のまま維持しつつ
	/// (双方向バインディングされたUIの表示を実際の値へ戻すために)通知だけを送りたい場合に使う。
	/// </summary>
	protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
