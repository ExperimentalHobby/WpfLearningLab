using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ContactManager.Models;

/// <summary>
/// 連絡先1件を表すエンティティ。編集フォームとの双方向バインディングのため<see cref="INotifyPropertyChanged"/>を実装する。
/// </summary>
public class Contact : INotifyPropertyChanged
{
	private string _name = string.Empty;
	private string _phoneNumber = string.Empty;
	private string _email = string.Empty;

	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;

	/// <summary>主キー。</summary>
	public int Id { get; set; }

	/// <summary>氏名。</summary>
	public string Name
	{
		get => _name;
		set
		{
			if (_name != value)
			{
				_name = value;
				OnPropertyChanged();
			}
		}
	}

	/// <summary>電話番号。</summary>
	public string PhoneNumber
	{
		get => _phoneNumber;
		set
		{
			if (_phoneNumber != value)
			{
				_phoneNumber = value;
				OnPropertyChanged();
			}
		}
	}

	/// <summary>メールアドレス。</summary>
	public string Email
	{
		get => _email;
		set
		{
			if (_email != value)
			{
				_email = value;
				OnPropertyChanged();
			}
		}
	}

	private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
