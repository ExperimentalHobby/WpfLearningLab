using System.Collections.ObjectModel;
using System.Windows.Input;
using AccessibleNoteApp.Models;
using AccessibleNoteApp.Services;

namespace AccessibleNoteApp.ViewModels;

/// <summary>
/// メモの一覧・作成・編集・削除を管理するViewModel。
/// </summary>
public class MainViewModel : ObservableObject
{
	private readonly IMemoRepository _repository;
	private Memo? _selectedMemo;
	private string _titleInput = string.Empty;
	private string _bodyInput = string.Empty;

	/// <summary>
	/// メモ一覧を空の状態で初期化する。永続化されたメモを読み込むには<see cref="Load"/>を呼ぶ。
	/// </summary>
	/// <param name="repository">メモの永続化を行う実装。</param>
	public MainViewModel(IMemoRepository repository)
	{
		_repository = repository;

		NewMemoCommand = new RelayCommand(NewMemo);
		SaveCommand = new RelayCommand(Save, () => !string.IsNullOrWhiteSpace(TitleInput));
		DeleteCommand = new RelayCommand(Delete, () => SelectedMemo is not null);
	}

	/// <summary>保存済みメモの一覧。</summary>
	public ObservableCollection<Memo> Memos { get; } = [];

	/// <summary>選択中のメモ。未選択の場合は<see langword="null"/>。</summary>
	public Memo? SelectedMemo
	{
		get => _selectedMemo;
		set
		{
			if (SetProperty(ref _selectedMemo, value))
			{
				TitleInput = value?.Title ?? string.Empty;
				BodyInput = value?.Body ?? string.Empty;
				OnPropertyChanged(nameof(SelectedIndex));
				((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>
	/// <see cref="Memos"/>内での選択Index(未選択の場合は-1)。
	/// <c>MemoListControl.SelectedIndex</c>との双方向バインディング用。
	/// </summary>
	public int SelectedIndex
	{
		get => _selectedMemo is null ? -1 : Memos.IndexOf(_selectedMemo);
		set => SelectedMemo = value >= 0 && value < Memos.Count ? Memos[value] : null;
	}

	/// <summary>編集中のタイトル。</summary>
	public string TitleInput
	{
		get => _titleInput;
		set
		{
			if (SetProperty(ref _titleInput, value))
			{
				((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
			}
		}
	}

	/// <summary>編集中の本文。</summary>
	public string BodyInput
	{
		get => _bodyInput;
		set => SetProperty(ref _bodyInput, value);
	}

	/// <summary>選択を解除し、新規メモの入力状態にするコマンド。</summary>
	public ICommand NewMemoCommand { get; }

	/// <summary>入力中の内容でメモを保存する(新規作成または更新)コマンド。</summary>
	public ICommand SaveCommand { get; }

	/// <summary>選択中のメモを削除するコマンド。</summary>
	public ICommand DeleteCommand { get; }

	/// <summary>
	/// リポジトリから保存済みの全メモを読み込み、<see cref="Memos"/>に反映する。
	/// </summary>
	public void Load()
	{
		Memos.Clear();
		foreach (var memo in _repository.LoadAll())
		{
			Memos.Add(memo);
		}
	}

	private void NewMemo()
	{
		SelectedMemo = null;
		TitleInput = string.Empty;
		BodyInput = string.Empty;
	}

	private void Save()
	{
		if (SelectedMemo is { } existing)
		{
			var updated = existing with { Title = TitleInput, Body = BodyInput, UpdatedAt = DateTime.Now };
			_repository.Save(updated);
			Memos[Memos.IndexOf(existing)] = updated;
			SelectedMemo = updated;
		}
		else
		{
			var created = new Memo(Guid.NewGuid().ToString("N"), TitleInput, BodyInput, DateTime.Now);
			_repository.Save(created);
			Memos.Add(created);
			SelectedMemo = created;
		}
	}

	private void Delete()
	{
		if (SelectedMemo is not { } memo)
		{
			return;
		}

		_repository.Delete(memo.Id);
		Memos.Remove(memo);
		SelectedMemo = null;
	}
}
