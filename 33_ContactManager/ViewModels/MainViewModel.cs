using System.Collections.ObjectModel;
using System.Windows.Input;
using ContactManager.Data;
using ContactManager.Models;

namespace ContactManager.ViewModels;

/// <summary>
/// 連絡先管理アプリのメインViewModel。
/// コンストラクタで<see cref="IContactRepository"/>を受け取る(DIコンテナから注入される)。
/// </summary>
public class MainViewModel : ObservableObject
{
	private readonly IContactRepository _repository;
	private Contact? _selectedContact;
	private string _newName = string.Empty;
	private string _newPhoneNumber = string.Empty;
	private string _newEmail = string.Empty;
	private string _editName = string.Empty;
	private string _editPhoneNumber = string.Empty;
	private string _editEmail = string.Empty;

	/// <summary>
	/// <see cref="MainViewModel"/>を初期化し、リポジトリから連絡先一覧を読み込む。
	/// </summary>
	public MainViewModel(IContactRepository repository)
	{
		_repository = repository;

		AddCommand = new RelayCommand(Add, CanAdd);
		UpdateCommand = new RelayCommand(Update, CanModifySelected);
		DeleteCommand = new RelayCommand(Delete, CanModifySelected);

		foreach (var contact in _repository.GetAll())
		{
			Contacts.Add(contact);
		}
	}

	/// <summary>登録済みの連絡先一覧。</summary>
	public ObservableCollection<Contact> Contacts { get; } = [];

	/// <summary>一覧で選択中の連絡先。</summary>
	public Contact? SelectedContact
	{
		get => _selectedContact;
		set
		{
			if (SetProperty(ref _selectedContact, value))
			{
				// 編集フォームはSelectedContactへ直接バインドせず、EditName等のコピーへ
				// バインドする。UpdateCommandを実行するまでは入力中の内容がエンティティに
				// 反映されないようにするため(EF Coreの変更追跡と相まって、Updateを押さずとも
				// 別のSaveChanges()で意図せず保存されてしまうのを防ぐ)。
				EditName = value?.Name ?? string.Empty;
				EditPhoneNumber = value?.PhoneNumber ?? string.Empty;
				EditEmail = value?.Email ?? string.Empty;
				((RelayCommand)UpdateCommand).RaiseCanExecuteChanged();
				((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>編集フォームの氏名。<see cref="UpdateCommand"/>実行時に<see cref="SelectedContact"/>へ反映される。</summary>
	public string EditName { get => _editName; set => SetProperty(ref _editName, value); }

	/// <summary>編集フォームの電話番号。<see cref="UpdateCommand"/>実行時に<see cref="SelectedContact"/>へ反映される。</summary>
	public string EditPhoneNumber { get => _editPhoneNumber; set => SetProperty(ref _editPhoneNumber, value); }

	/// <summary>編集フォームのメールアドレス。<see cref="UpdateCommand"/>実行時に<see cref="SelectedContact"/>へ反映される。</summary>
	public string EditEmail { get => _editEmail; set => SetProperty(ref _editEmail, value); }

	/// <summary>新規追加フォームの氏名。</summary>
	public string NewName
	{
		get => _newName;
		set
		{
			if (SetProperty(ref _newName, value))
			{
				((RelayCommand)AddCommand).RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>新規追加フォームの電話番号。</summary>
	public string NewPhoneNumber { get => _newPhoneNumber; set => SetProperty(ref _newPhoneNumber, value); }

	/// <summary>新規追加フォームのメールアドレス。</summary>
	public string NewEmail { get => _newEmail; set => SetProperty(ref _newEmail, value); }

	/// <summary>連絡先を新規追加するコマンド。</summary>
	public ICommand AddCommand { get; }

	/// <summary>選択中の連絡先を更新するコマンド。</summary>
	public ICommand UpdateCommand { get; }

	/// <summary>選択中の連絡先を削除するコマンド。</summary>
	public ICommand DeleteCommand { get; }

	private bool CanAdd() => !string.IsNullOrWhiteSpace(NewName);

	private bool CanModifySelected() => SelectedContact is not null;

	private void Add()
	{
		var contact = new Contact { Name = NewName, PhoneNumber = NewPhoneNumber, Email = NewEmail };
		_repository.Add(contact);
		Contacts.Add(contact);

		NewName = string.Empty;
		NewPhoneNumber = string.Empty;
		NewEmail = string.Empty;
	}

	private void Update()
	{
		if (SelectedContact is null)
		{
			return;
		}

		SelectedContact.Name = EditName;
		SelectedContact.PhoneNumber = EditPhoneNumber;
		SelectedContact.Email = EditEmail;
		_repository.Update(SelectedContact);
	}

	private void Delete()
	{
		if (SelectedContact is null)
		{
			return;
		}
		_repository.Delete(SelectedContact.Id);
		Contacts.Remove(SelectedContact);
		SelectedContact = null;
	}
}
